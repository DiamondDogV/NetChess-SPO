using Chess.Domain.Entities;
using Chess.Models;
using Chess.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Chess.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IActiveRoomService _roomService;
        public GameController(IActiveRoomService roomService) => _roomService = roomService;

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

            var entity = new ActiveRoom
            {
                Name = model.RoomName,
                Elo = 10,
                Special = "sd",
                Token = token,
                IsWhite = model.IsWhite,
            };

            var resp = await _roomService.CreateForTabAsync(entity);
            if (resp.Code != Domain.Enums.Enums.ResponceCodes.Ok)
                return BadRequest("Не удалось создать комнату");

            ViewBag.Token = token;
            return View("OpeningTab");
        }

        [HttpGet]
        public async Task<IActionResult> Board(string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest();

            var resp = await _roomService.GetByTokenAsync(token);
            if (resp.Code != Domain.Enums.Enums.ResponceCodes.Ok)
                return NotFound("Комната не найдена или уже закрыта");

            var room = resp.Data;
            var model = new ChessBoardModel
            {
                RoomName = room.Name,
                IsWhite = room.IsWhite,
                IsBlack = !room.IsWhite,
            };
            ViewBag.Token = token;
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
    }
}
