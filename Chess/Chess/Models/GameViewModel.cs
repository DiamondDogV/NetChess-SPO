using Chess.Domain.Interfaces;
using static Chess.Domain.Enums.Enums;

namespace Chess.Models
{
    public class GameViewModel
    {
        public string RoomName { get; set; }
        public string Token { get; set; }
        public IChess[,] Board { get; set; }
        public MovesType[,] MovesMatrix { get; set; }
        public bool IsWhite { get; set; }
        public string Role { get; set; }
    }
}
