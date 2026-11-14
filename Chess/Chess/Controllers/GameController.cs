using Chess.Domain.DTO;
using Chess.Domain.Entities;
using Chess.Domain.Helpers;
using Chess.Domain.Interfaces;
using Chess.Models;
using Chess.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using static Chess.Domain.Enums.Enums;

namespace Chess.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IActiveRoomService _roomService;
        private readonly IUserService _userService;
        public GameController(IActiveRoomService roomService, IUserService userService) => (_roomService, _userService) = (roomService, userService);

        public IActionResult CreateGame() => View();

        public async Task<IActionResult> FindRoom()
        {
            var rooms = (await _roomService.GetAllAsync()).Data?.ToList() ?? new();
            var model = rooms.Select(r => new ActiveRoomModel { Name = r.Name, Elo = r.Elo }).ToList();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlayGame(ChessBoardModel model)
        {
            if (model.SelectedColor == "Random")
            {
                var rand = new Random();
                model.IsWhite = rand.Next(0, 2) == 1;
                model.IsBlack = !model.IsWhite;
            }
            else
            {
                model.IsWhite = model.SelectedColor == "White";
                model.IsBlack = !model.IsWhite;
            }

            var token = Guid.NewGuid().ToString("N");
            var board = ChessHelper.CreateStartBoard();
            string boardData = ChessHelper.Serialize(board);

            var entity = new ActiveRoom
            {
                Name = model.RoomName,
                Elo = 100,
                Special = boardData,
                Token = token,
                IsWhite = model.IsWhite,
                IsWhiteTurn = true, // Белые всегда начинают
                IsMate = false,
                CreatedAt = DateTime.UtcNow,
                LastHeartbeat = DateTime.UtcNow
            };

            await _roomService.CreateForTabAsync(entity);

            HttpContext.Session.SetString($"Role_{token}", "Owner");
            ViewBag.Token = token;
            return View("OpeningTab");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(string roomName)
        {
            var rooms = (await _roomService.GetAllAsync()).Data;
            var room = rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null) return NotFound("Комната не найдена");

            // Гость
            HttpContext.Session.SetString($"Role_{room.Token}", "Guest");
            return RedirectToAction("Board", new { token = room.Token });
        }

        [HttpGet]
        public async Task<IActionResult> Board(string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest();

            var resp = await _roomService.GetByTokenAsync(token);
            if (resp.Code != Domain.Enums.Enums.ResponceCodes.Ok)
                return NotFound("Комната не найдена");

            var room = resp.Data;
            var board = ChessHelper.Deserialize(room.Special);

            string role = HttpContext.Session.GetString($"Role_{token}") ?? "Owner";
            // Если Owner - играет за room.IsWhite. Если Guest - за противоположный цвет.
            bool playerIsWhite = (role == "Guest") ? !room.IsWhite : room.IsWhite;

            var model = new GameViewModel
            {
                RoomName = room.Name,
                Token = token,
                Board = board,
                MovesMatrix = new MovesType[8, 8],
                IsWhite = playerIsWhite,
                Role = role
            };

            return View("PlayGame", model);
        }

        [HttpPost]
        public async Task<IActionResult> SelectPiece([FromBody] MoveRequest req)
        {
            var room = (await _roomService.GetByTokenAsync(req.Token)).Data;
            if (room == null) return BadRequest("Комната не найдена");
            if (room.IsMate) return BadRequest("Игра окончена. Мат!");

            var board = ChessHelper.Deserialize(room.Special);

            string role = HttpContext.Session.GetString($"Role_{req.Token}") ?? "Owner";
            bool playerIsWhite = (role == "Guest") ? !room.IsWhite : room.IsWhite;

            // ПРОВЕРКА ОЧЕРЕДИ
            if (room.IsWhiteTurn != playerIsWhite)
                return BadRequest("Сейчас не ваш ход!");

            var piece = board[req.FromY, req.FromX];

            if (piece.ChessType == CMT.None)
                return BadRequest("Пустая клетка");

            // ПРОВЕРКА ЦВЕТА
            if (piece.IsBlack == playerIsWhite)
                return BadRequest("Это не ваша фигура!");

            piece.ShowMoves(in board, out MovesType[,] movesMatrix);
            return Json(new { moves = ConvertMovesToJson(movesMatrix) });
        }


        [HttpPost]
        public async Task<IActionResult> MakeMove([FromBody] MoveRequest req)
        {
            var room = (await _roomService.GetByTokenAsync(req.Token)).Data;
            if (room == null) return BadRequest("Комната не найдена");
            if (room.IsMate) return BadRequest("Игра окончена. Мат!");

            var board = ChessHelper.Deserialize(room.Special);

            string role = HttpContext.Session.GetString($"Role_{req.Token}") ?? "Owner";
            bool playerIsWhite = (role == "Guest") ? !room.IsWhite : room.IsWhite;

            // ПРОВЕРКА ОЧЕРЕДИ
            if (room.IsWhiteTurn != playerIsWhite)
                return BadRequest("Сейчас не ваш ход!");

            var selectedPiece = board[req.FromY, req.FromX];

            if (selectedPiece.ChessType == CMT.None || selectedPiece.IsBlack == playerIsWhite)
                return BadRequest("Неверная фигура");

            IChess[,] m2 = new IChess[8, 8];
            ChessHelper.CopyMatrix(board, ref m2);

            bool isMoveMade = false;
            Point targetPoint = new Point(req.ToX, req.ToY);

            if (board[req.ToY, req.ToX].ChessType == CMT.None && selectedPiece.CanMove(board, targetPoint))
            {
                selectedPiece.Position = targetPoint;
                board[req.ToY, req.ToX] = selectedPiece;
                board[req.FromY, req.FromX] = new Cell();
                isMoveMade = true;
            }
            else if (board[req.ToY, req.ToX].ChessType != CMT.None && board[req.ToY, req.ToX].IsBlack != selectedPiece.IsBlack && selectedPiece.CanBeat(in board, targetPoint))
            {
                selectedPiece.Position = targetPoint;
                board[req.ToY, req.ToX] = selectedPiece;
                board[req.FromY, req.FromX] = new Cell();
                isMoveMade = true;
            }

            if (!isMoveMade)
                return BadRequest("Недопустимый ход");

            // ВРЕМЕННО СТАВИМ IsMy, ЧТОБЫ ТВОЙ СТАРЫЙ isMat СРАБОТАЛ
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board[y, x].ChessType != CMT.None)
                    {
                        // Тот, кто только что походил, становится "врагом" для проверки мата
                        board[y, x].IsMy = (board[y, x].IsBlack != playerIsWhite);
                    }
                }
            }

            bool isMate = ChessHelper.isMat(board, new Point(-1, -1));

            room.Special = ChessHelper.Serialize(board);
            room.IsMate = isMate;

            if (!isMate)
            {
                room.IsWhiteTurn = !room.IsWhiteTurn; // Передаем ход
            }

            await _roomService.UpdateAsync(room);

            return Json(new { success = true, isMate = isMate, message = isMate ? "Шах и мат!" : "" });
        }

        [HttpGet]
        public async Task<IActionResult> GetBoardState(string token)
        {
            var room = (await _roomService.GetByTokenAsync(token)).Data;
            if (room == null) return NotFound();
            return Json(new { state = room.Special });
        }

        [HttpPost]
        public async Task<IActionResult> Heartbeat([FromBody] HeartbeatDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Token)) return BadRequest();
            var resp = await _roomService.HeartbeatAsync(dto.Token);
            return resp.Data ? Ok() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CloseRoom([FromBody] HeartbeatDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Token)) return BadRequest();
            await _roomService.CloseAsync(dto.Token);
            return Ok();
        }

        public class HeartbeatDto { public string Token { get; set; } }

        private void ApplyIsMy(IChess[,] board, bool playerIsWhite)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board[y, x].ChessType != CMT.None)
                    {
                        board[y, x].IsMy = (board[y, x].IsBlack != playerIsWhite);
                    }
                }
            }
        }

        private List<List<int>> ConvertMovesToJson(MovesType[,] moves)
        {
            var result = new List<List<int>>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (moves[y, x] != MovesType.None)
                    {
                        result.Add(new List<int> { x, y, (int)moves[y, x] });
                    }
                }
            }
            return result;
        }
    }
}