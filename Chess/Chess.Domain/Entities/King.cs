using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;

namespace Chess.Domain.Entities
{
    public class King : IChess
    {
        public CMT ChessType { get; set; } = CMT.King;
        public bool IsBlack { get; set; }

        public byte Price { get; set; } = 0;
        public bool IsCanJump { get; set; } = false;
        public Point Position { get; set; }
        public bool IsFirstMove { get; set; }
        public bool IsMy { get; set; }

        public King(bool isBlask, Point position, bool isMy, bool isFirstMove)
        {
            IsFirstMove = isFirstMove;
            IsBlack = isBlask;
            Position = position;
            IsMy = isMy;
        }

        public bool CanMakeRokir(in IChess[,] matrix, Point to)
        {
            if (IsFirstMove && (matrix[to.Y, to.X] as Roock)?.IsFirstMove == true)
            {
                if (Position.X - to.X <= 0)
                {
                    if (matrix[Position.Y, Position.X + 1].ChessType == CMT.None
                        && matrix[Position.Y, Position.X + 2].ChessType == CMT.None)
                    {
                        return true;
                    }
                }
                else
                {
                    if (matrix[Position.Y, Position.X - 3].ChessType == CMT.None && matrix[Position.Y, Position.X - 1].ChessType == CMT.None
                       && matrix[Position.Y, Position.X - 2].ChessType == CMT.None)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckStillExist(in IChess[,] matrix, Point emulation)
        {
            int targetX = (emulation.X != -1) ? emulation.X : Position.X;
            int targetY = (emulation.Y != -1) ? emulation.Y : Position.Y;
            Point targetPoint = new Point(targetX, targetY);

            /*using (FileStream file = new("debug", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter wr = new(file))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            matrix[i, j].Position = new Point(j, i);
                            if (matrix[i, j].ChessType != CMT.None && !matrix[i, j].IsMy)
                            {
                                wr.Write("[" + i + " " + j + " " + matrix[i, j].ChessType + "]");
                            }
                            else
                                wr.Write("[]");
                        }
                        wr.Write("\n");
                    }

                }
            }*/
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (matrix[i, j].ChessType != CMT.None && !matrix[i, j].IsMy)
                    {
                        matrix[i, j].Position = new Point(j, i);
                        if (matrix[i, j].CanBeat(matrix, targetPoint))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
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
            movesMatrix = new MovesType[8, 8];
            CheckMove(Position.Y - 1, Position.X + 1, ref movesMatrix, matrix, this);
            CheckMove(Position.Y - 1, Position.X - 1, ref movesMatrix, matrix, this);
            CheckMove(Position.Y - 1, Position.X, ref movesMatrix, matrix, this);
            CheckMove(Position.Y + 1, Position.X - 1, ref movesMatrix, matrix, this);
            CheckMove(Position.Y + 1, Position.X + 1, ref movesMatrix, matrix, this);
            CheckMove(Position.Y + 1, Position.X, ref movesMatrix, matrix, this);
            CheckMove(Position.Y, Position.X - 1, ref movesMatrix, matrix, this);
            CheckMove(Position.Y, Position.X + 1, ref movesMatrix, matrix, this);

            if (CanMakeRokir(matrix, new Point(7, 7)))
            {
                movesMatrix[7, 7] = MovesType.Move;
            }
            if (CanMakeRokir(matrix, new Point(0, 7)))
            {
                movesMatrix[7, 0] = MovesType.Move;
            }
        }

        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            if ((Position.Y - 1 == to.Y) && (Position.X - 1 == to.X) ||
              (Position.Y - 1 == to.Y) && (Position.X == to.X) ||
              (Position.Y - 1 == to.Y) && (Position.X + 1 == to.X) ||
              (Position.Y + 1 == to.Y) && (Position.X - 1 == to.X) ||
              (Position.Y + 1 == to.Y) && (Position.X == to.X) ||
              (Position.Y + 1 == to.Y) && (Position.X + 1 == to.X) ||
              (Position.Y == to.Y) && (Position.X - 1 == to.X) ||
              (Position.Y == to.Y) && (Position.X + 1 == to.X))
            {
                IsFirstMove = false;
                return (true);
            }
            return (false);
        }

        public bool CanMove(in IChess[,] matrix, Point to)
        {
            if ((Position.Y - 1 == to.Y) && (Position.X - 1 == to.X) ||
               (Position.Y - 1 == to.Y) && (Position.X == to.X) ||
               (Position.Y - 1 == to.Y) && (Position.X + 1 == to.X) ||
               (Position.Y + 1 == to.Y) && (Position.X - 1 == to.X) ||
               (Position.Y + 1 == to.Y) && (Position.X == to.X) ||
               (Position.Y + 1 == to.Y) && (Position.X + 1 == to.X) ||
               (Position.Y == to.Y) && (Position.X - 1 == to.X) ||
               (Position.Y == to.Y) && (Position.X + 1 == to.X))
            {
                IsFirstMove = false;
                return (true);
            }
            return (false);
        }
    }
}
