using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;
using static Chess.Domain.Helpers.ChessHelper;

namespace Chess.Domain.Entities
{
    public class Pawn : IChess
    {
        public CMT ChessType { get; set; } = CMT.Pawn;
        public bool IsBlack { get; set; }

        public byte Price { get; set; } = 1;
        public bool IsCanJump { get; set; } = false;
        public Point Position { get; set; }
        bool isFirstMove = true;
        public bool IsMy { get; set; }



        public Pawn(bool isBlask, Point position, bool isMy)
        {
            IsBlack = isBlask;
            Position = position;
            IsMy = isMy;
        }

        public bool CanMove(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            if (isFirstMove)
            {
                if (((Position.Y - 2 == to.Y) && matrix[Position.Y - 2, Position.X].ChessType == CMT.None) && matrix[Position.Y - 1, Position.X].ChessType == CMT.None
                    && Position.X == to.X)
                {
                    isFirstMove = false;
                    return true;
                }
            }
            if ((Position.Y - 1 == to.Y) && Position.X == to.X)
            {
                isFirstMove = false;
                return true;
            }
            return false;
        }

        public void ShowMoves(in IChess[,] matrix, out MovesType[,] movesMatrix)
        {
            King king = FindMyKing(matrix);
            movesMatrix = new MovesType[8, 8];
            if (Position.Y - 1 >= 0 && Position.Y - 1 <= 7 && Position.X >= 0 && Position.X <= 7)
            {
                if (isFirstMove && matrix[Position.Y - 2, Position.X].ChessType == CMT.None && matrix[Position.Y - 1, Position.X].ChessType == CMT.None
                       )
                {
                    movesMatrix[Position.Y - 2, Position.X] = MovesType.Move;
                }
                if (matrix[Position.Y - 1, Position.X].ChessType == CMT.None)
                {
                    movesMatrix[Position.Y - 1, Position.X] = MovesType.Move;
                }
                if (Position.Y - 1 >= 0 && Position.Y - 1 <= 7 && Position.X - 1 >= 0 && Position.X - 1 <= 7)
                {
                    if (!matrix[Position.Y - 1, Position.X - 1].IsMy && matrix[Position.Y - 1, Position.X - 1].ChessType != CMT.None)
                    {
                        movesMatrix[Position.Y - 1, Position.X - 1] = MovesType.Beat;
                    }
                }
                if (Position.Y - 1 >= 0 && Position.Y - 1 <= 7 && Position.X + 1 >= 0 && Position.X + 1 <= 7)
                {
                    if (!matrix[Position.Y - 1, Position.X + 1].IsMy && matrix[Position.Y - 1, Position.X + 1].ChessType != CMT.None)
                    {
                        movesMatrix[Position.Y - 1, Position.X + 1] = MovesType.Beat;
                    }
                }
            }
        }


        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            if (IsMy)
            {
                if (((Position.Y - 1 == to.Y) && (Position.X - 1 == to.X))
                        || ((Position.Y - 1 == to.Y) && (Position.X + 1 == to.X)))
                {
                    return (true);
                }
            }
            else
            {
                if (((Position.Y + 1 == to.Y) && (Position.X - 1 == to.X))
                        || ((Position.Y + 1 == to.Y) && (Position.X + 1 == to.X)))
                {
                    return (true);
                }
            }
            return (false);
        }
    }
}
