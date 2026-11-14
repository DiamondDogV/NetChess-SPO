using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;
using static Chess.Domain.Helpers.ChessHelper;

namespace Chess.Domain.Entities
{
    public class Ferz : IChess
    {
        public CMT ChessType { get; set; } = CMT.Ferz;
        public bool IsBlack { get; set; }

        public byte Price { get; set; } = 8;
        public bool IsCanJump { get; set; } = false;
        public Point Position { get; set; }
        public bool IsMy { get; set; }

        public bool IsFirstMove { get; set; }

        public Ferz(bool isBlask, Point position, bool isMy,bool isFirstMove)
        {
            IsFirstMove = isFirstMove;
            IsBlack = isBlask;
            Position = position;
            IsMy = isMy;
        }

        public void ShowMoves(in IChess[,] matrix, out MovesType[,] movesMatrix)
        {
            movesMatrix = new MovesType[8, 8];
            King king = FindMyKing(matrix);
            for (int i = Position.Y + 1; i < 8; i++)
            {
                if (matrix[i, Position.X].ChessType != CMT.None)
                {
                    if (!matrix[i, Position.X].IsMy)
                        movesMatrix[i, Position.X] = MovesType.Beat;
                    break;
                }
                else if (matrix[i, Position.X].ChessType == CMT.None)
                {
                    movesMatrix[i, Position.X] = MovesType.Move;
                }
            }
            for (int i = Position.Y - 1; i >= 0; i--)
            {
                if (matrix[i, Position.X].ChessType != CMT.None)
                {
                    if (!matrix[i, Position.X].IsMy)
                        movesMatrix[i, Position.X] = MovesType.Beat;
                    break;
                }
                else if (matrix[i, Position.X].ChessType == CMT.None)
                {
                    movesMatrix[i, Position.X] = MovesType.Move;
                }
            }
            for (int i = Position.X + 1; i < 8; i++)
            {
                if (matrix[Position.Y, i].ChessType != CMT.None)
                {
                    if (!matrix[Position.Y, i].IsMy)
                        movesMatrix[Position.Y, i] = MovesType.Beat;
                    break;
                }
                else if (matrix[Position.Y, i].ChessType == CMT.None)
                {
                    movesMatrix[Position.Y, i] = MovesType.Move;
                }
            }
            for (int i = Position.X - 1; i >= 0; i--)
            {
                if (matrix[Position.Y, i].ChessType != CMT.None)
                {
                    if (!matrix[Position.Y, i].IsMy)
                        movesMatrix[Position.Y, i] = MovesType.Beat;
                    break;
                }
                else if (matrix[Position.Y, i].ChessType == CMT.None)
                {
                    movesMatrix[Position.Y, i] = MovesType.Move;
                }
            }
            for (int i = Position.Y + 1, j = Position.X + 1; i < 8 && j < 8; i++, j++)
            {
                if (matrix[i, j].ChessType != CMT.None)
                {
                    if (!matrix[i, j].IsMy)
                        movesMatrix[i, j] = MovesType.Beat;
                    break;
                }
                else if (matrix[i, j].ChessType == CMT.None)
                {
                    movesMatrix[i, j] = MovesType.Move;
                }
            }
            for (int i = Position.Y - 1, j = Position.X + 1; i >= 0 && j < 8; i--, j++)
            {
                if (matrix[i, j].ChessType != CMT.None)
                {
                    if (!matrix[i, j].IsMy)
                        movesMatrix[i, j] = MovesType.Beat;
                    break;
                }
                else if (matrix[i, j].ChessType == CMT.None)
                {
                    movesMatrix[i, j] = MovesType.Move;
                }
            }
            for (int i = Position.Y + 1, j = Position.X - 1; i < 8 && j >= 0; i++, j--)
            {
                if (matrix[i, j].ChessType != CMT.None)
                {
                    if (!matrix[i, j].IsMy)
                        movesMatrix[i, j] = MovesType.Beat;
                    break;
                }
                else if (matrix[i, j].ChessType == CMT.None)
                {
                    movesMatrix[i, j] = MovesType.Move;
                }
            }
            for (int i = Position.Y - 1, j = Position.X - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (matrix[i, j].ChessType != CMT.None)
                {
                    if (!matrix[i, j].IsMy)
                        movesMatrix[i, j] = MovesType.Beat;
                    break;
                }
                else if (matrix[i, j].ChessType == CMT.None)
                {
                    movesMatrix[i, j] = MovesType.Move;
                }
            }
        }

        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            for (int i = Position.Y + 1, j = Position.X + 1; i < 8 && j < 8; i++, j++)
            {
                if (matrix[i, j].ChessType != CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }
                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            for (int i = Position.Y - 1, j = Position.X + 1; i >= 0 && j < 8; i--, j++)
            {
                if (matrix[i, j].ChessType != CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }
                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }

            for (int i = Position.Y + 1, j = Position.X - 1; i < 8 && j >= 0; i++, j--)
            {
                if (matrix[i, j].ChessType != CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }

                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            for (int i = Position.Y - 1, j = Position.X - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (matrix[i, j].ChessType != CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }

                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            for (int i = Position.Y + 1; i < 8; i++)
            {
                if (matrix[i, Position.X].ChessType != CMT.None && (i == to.Y && Position.X == to.X))
                {
                    return (true);
                }

                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }
            for (int i = Position.Y - 1; i >= 0; i--)
            {
                if (matrix[i, Position.X].ChessType != CMT.None && (i == to.Y && Position.X == to.X))
                {
                    return (true);
                }
                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }

            for (int i = Position.X + 1; i < 8; i++)
            {
                if (matrix[Position.Y, i].ChessType != CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            for (int i = Position.X - 1; i >= 0; i--)
            {
                if (matrix[Position.Y, i].ChessType != CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            return (false);
        }

        public bool CanMove(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            bool isBeatMyKing = false, isBeatKing = false;
            for (int i = Position.Y + 1; i < 8; i++)
            {
                if (matrix[i, Position.X].ChessType == CMT.None && (i == to.Y && Position.X == to.X))
                {
                    return (true);
                }
                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }
            for (int i = Position.Y - 1; i >= 0; i--)
            {
                if (matrix[i, Position.X].ChessType == CMT.None && (i == to.Y && Position.X == to.X))
                {
                    return (true);
                }
                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }

            for (int i = Position.X + 1; i < 8; i++)
            {
                if (matrix[Position.Y, i].ChessType == CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            for (int i = Position.X - 1; i >= 0; i--)
            {
                if (matrix[Position.Y, i].ChessType == CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            for (int i = Position.Y + 1, j = Position.X + 1; i < 8 && j < 8; i++, j++)
            {
                if (matrix[i, j].ChessType == CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }
                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            for (int i = Position.Y - 1, j = Position.X + 1; i >= 0 && j < 8; i--, j++)
            {
                if (matrix[i, j].ChessType == CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }
                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            for (int i = Position.Y + 1, j = Position.X - 1; i < 8 && j >= 0; i++, j--)
            {
                if (matrix[i, j].ChessType == CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }
                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            for (int i = Position.Y - 1, j = Position.X - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (matrix[i, j].ChessType == CMT.None && (i == to.Y && j == to.X))
                {
                    return (true);
                }
                if (matrix[i, j].ChessType != CMT.None && !(i == to.Y && j == to.X))
                    break;
            }
            return (false);
        }
    }
}
