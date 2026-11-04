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
        public CMT ChessType { get; init; } = CMT.None;
        public bool IsBlack { get; init; }

        public byte Price { get; init; }
        public bool IsCanJump { get; init; }
        public bool IsMy { get; init; }
        public Point Position { get; set; }

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
