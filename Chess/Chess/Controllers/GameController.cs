using Chess.Domain.Entities;
using Chess.Domain.Helpers;
using Chess.Models;
using Chess.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Chess.Domain.Enums.Enums;

[Authorize]
public class GameController : Controller
{
    private readonly IActiveRoomService _roomService;
    private readonly IUserService _userService;

    public GameController(IActiveRoomService roomService, IUserService userService)
        => (_roomService, _userService) = (roomService, userService);

    public IActionResult CreateGame() => View();

    public async Task<IActionResult> FindRoom()
    {
        var rooms = (await _roomService.GetAllAsync()).Data?.ToList() ?? new();
        var model = rooms
            .Where(r => r.GuestUserId == null && !r.IsMate)
            .Select(r => new ActiveRoomModel { Name = r.Name, Elo = r.Elo })
            .ToList();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlayGame(ChessBoardModel model)
    {
        bool isWhite = model.SelectedColor == "Random"
            ? new Random().Next(0, 2) == 1
            : model.SelectedColor == "White";

        var token = Guid.NewGuid().ToString("N");

        long? userId = null;
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (long.TryParse(userIdStr, out var parsedId)) userId = parsedId;

        var entity = new ActiveRoom
        {
            Name = model.RoomName,
            Elo = 100,
            Token = token,
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow,
            LastHeartbeat = DateTime.UtcNow,
            IsWhite = isWhite,
            IsWhiteTurn = true,
            IsMate = false,
            BoardState = ChessHelper.Serialize(ChessHelper.CreateStartBoard())
        };

        await _roomService.CreateForTabAsync(entity);

        ViewBag.Token = token;
        return View("OpeningTab");
    }

    [HttpGet]
    public async Task<IActionResult> JoinRoom(string roomName)
    {
        var rooms = (await _roomService.GetAllAsync()).Data;
        var room = rooms.FirstOrDefault(r => r.Name == roomName && r.GuestUserId == null);
        if (room == null) return NotFound("Комната не найдена");
        return RedirectToAction("Board", new { token = room.Token });
    }

    [HttpGet]
    public async Task<IActionResult> Board(string token)
    {
        if (string.IsNullOrEmpty(token)) return BadRequest();

        var resp = await _roomService.GetByTokenAsync(token);
        if (resp.Code != ResponceCodes.Ok)
            return NotFound("Комната не найдена");

        var room = resp.Data;

        var emptyBoard = ChessHelper.CreateStartBoard();
        var model = new GameViewModel
        {
            RoomName = room.Name,
            Token = token,
            Board = emptyBoard,
            MovesMatrix = new MovesType[8, 8],
            IsWhite = room.IsWhite,
            Role = "Player"
        };
        return View("PlayGame", model);
    }
}