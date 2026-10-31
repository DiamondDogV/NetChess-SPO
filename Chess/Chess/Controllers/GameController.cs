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
        IActiveRoomService roomService;
        public GameController(IActiveRoomService roomService) { this.roomService = roomService; }

        public IActionResult CreateGame()
        {
            return View();
        }

        public async Task<IActionResult> FindRoom()
        {
            List<ActiveRoom> rooms = (await roomService.GetAllAsync()).Data.ToList();
            List<ActiveRoomModel> model = new();
            for (int i = 0; i < rooms.Count; i++)
            {
                model.Add(new() { Elo = rooms[i].Elo, Name = rooms[i].Name });
            }
            return View(model);
        }

        public async Task<IActionResult> PlayGame(ChessBoardModel model) 
        {
            if (model.SelectedColor == "White")
            {
                model.IsWhite = true;
            }
            if (model.SelectedColor == "Black")
            {
                model.IsBlack = true;
            }
            if (model.SelectedColor == "Random")
            {
                Random rand = new Random();
                model.IsWhite = rand.Next(0, 2) == 1;
                model.IsBlack = !model.IsWhite;
            }

            await roomService.CreateAsync(new() 
            {
                Elo = 10,
                Name = model.RoomName,
                Special = "sd"
            });

            return View(model);
        }
    }
}
