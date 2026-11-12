using Chess.Domain.Entities;
using Chess.Domain.Helpers;
using Chess.Domain.Interfaces;
using Chess.Models;
using Chess.Service.Implementations;
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

        [HttpGet]
        public async Task<IActionResult> JoinRoom(string roomName)
        {
            var rooms = (await _roomService.GetAllAsync()).Data;
            var room = rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null) return NotFound("Комната не найдена");

            HttpContext.Session.SetString($"Role_{room.Token}", "Guest");

            return RedirectToAction("Board", new { token = room.Token });
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

            int playerElo = 100;
            string userIdStr = HttpContext.Session.GetString("UserId");
            if (long.TryParse(userIdStr, out long userId))
            {
                var userResp = await _userService.GetByIdAsync(userId);
                if (userResp.Code == Domain.Enums.Enums.ResponceCodes.Ok && userResp.Data != null)
                {
                    playerElo = userResp.Data.EloRating;
                }
            }

            var entity = new ActiveRoom
            {
                Name = model.RoomName,
                Elo = playerElo,
                Special = boardData,
                Token = token,
                IsWhite = model.IsWhite,
            };

            var resp = await _roomService.CreateForTabAsync(entity);
            if (resp.Code != Domain.Enums.Enums.ResponceCodes.Ok)
                return BadRequest("Не удалось создать комнату");

            HttpContext.Session.SetString($"Role_{token}", "Owner");

            ViewBag.Token = token;
            return View("OpeningTab");
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
            bool playerIsWhite = room.IsWhite;

            if (role == "Guest")
            {
                playerIsWhite = !room.IsWhite;
            }

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

            var model = new GameViewModel
            {
                RoomName = room.Name,
                Token = token,
                Board = board,
                MovesMatrix = new MovesType[8, 8],
                IsWhite = playerIsWhite
            };

            return View("PlayGame", model);
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
        private void SwitchTurn(IChess[,] board)
        {
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if (board[y, x].ChessType != CMT.None)
                        board[y, x].IsMy = !board[y, x].IsMy;
        }
    }
}
