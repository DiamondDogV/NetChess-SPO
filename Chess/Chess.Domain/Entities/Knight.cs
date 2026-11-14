using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;
using static Chess.Domain.Helpers.ChessHelper;

namespace Chess.Domain.Entities
{
    public class Knight : IChess
    {
        public CMT ChessType { get; set; } = CMT.Knight;
        public bool IsBlack { get; set; }

        public byte Price { get; set; } = 3;
        public bool IsCanJump { get; set; } = true;
        public Point Position { get; set; }
        public bool IsMy { get; set; }

        public bool IsFirstMove { get; set; }

        public Knight(bool isBlask, Point position, bool isMy, bool isFirstMove)
        {
            IsFirstMove = isFirstMove;
            IsBlack = isBlask;
            Position = position;
            IsMy = isMy;
        }

        public bool CanMove(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            if (((Position.X + 1) == to.X && Position.Y - 2 == to.Y) ||
                ((Position.X - 1) == to.X && Position.Y - 2 == to.Y) ||
                ((Position.X + 1) == to.X && Position.Y + 2 == to.Y) ||
                ((Position.X - 1) == to.X && Position.Y + 2 == to.Y) ||
                ((Position.X - 2) == to.X && Position.Y + 1 == to.Y) ||
                ((Position.X - 2) == to.X && Position.Y - 1 == to.Y) ||
                ((Position.X + 2) == to.X && Position.Y - 1 == to.Y) ||
                ((Position.X + 2) == to.X && Position.Y + 1 == to.Y))
            {
                return true;
            }
            return false;
        }

        void CheckMove(int Y, int X, ref MovesType[,] movesMatrix, in IChess[,] matrix, King king)
        {
            if (Y >= 0 && Y <= 7 && X >= 0 && X <= 7)
            {
                movesMatrix[Y, X] = matrix[Y, X].ChessType == CMT.None ? MovesType.Move :
                     (matrix[Y, X].ChessType != CMT.None && !matrix[Y, X].IsMy) ?
                     MovesType.Beat : MovesType.None;
            }
        }

        public void ShowMoves(in IChess[,] matrix, out MovesType[,] movesMatrix)
        {
            King king = FindMyKing(matrix);
            movesMatrix = new MovesType[8, 8];
            CheckMove(Position.Y - 2, Position.X + 1, ref movesMatrix, matrix, king);
            CheckMove(Position.Y - 2, Position.X - 1, ref movesMatrix, matrix, king);
            CheckMove(Position.Y + 2, Position.X + 1, ref movesMatrix, matrix, king);
            CheckMove(Position.Y + 2, Position.X - 1, ref movesMatrix, matrix, king);
            CheckMove(Position.Y - 1, Position.X - 2, ref movesMatrix, matrix, king);
            CheckMove(Position.Y + 1, Position.X - 2, ref movesMatrix, matrix, king);
            CheckMove(Position.Y + 1, Position.X + 2, ref movesMatrix, matrix, king);
            CheckMove(Position.Y - 1, Position.X + 2, ref movesMatrix, matrix, king);
        }

        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            if (((Position.X + 1) == to.X && Position.Y - 2 == to.Y) ||
                ((Position.X - 1) == to.X && Position.Y - 2 == to.Y) ||
                ((Position.X + 1) == to.X && Position.Y + 2 == to.Y) ||
                ((Position.X - 1) == to.X && Position.Y + 2 == to.Y) ||
                ((Position.X - 2) == to.X && Position.Y + 1 == to.Y) ||
                ((Position.X - 2) == to.X && Position.Y - 1 == to.Y) ||
                ((Position.X + 2) == to.X && Position.Y - 1 == to.Y) ||
                ((Position.X + 2) == to.X && Position.Y + 1 == to.Y))
            {
                return (true);
            }
            return (false);
        }
    }
}
