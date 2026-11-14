using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;
using System.Drawing;

namespace Chess.Domain.Entities
{
    public class Cell : IChess
    {
        public CMT ChessType { get; set; } = CMT.None;
        public bool IsBlack { get; set; }

        public byte Price { get; set; }
        public bool IsCanJump { get; set; }
        public bool IsMy { get; set; }
        public Point Position { get; set; }
        public bool IsFirstMove { get; set; }

        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            return (false);
        }

        public bool CanMove(in IChess[,] matrix, Point to)
        {
            return false;
        }

        public void ShowMoves(in IChess[,] matrix, out MovesType[,] movesMatrix)
        {
            movesMatrix = new MovesType[8, 8];
        }
    }
}
