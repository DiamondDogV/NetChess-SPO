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
    public class Roock : IChess
    {
        public CMT ChessType { get; init; } = CMT.Roock;
        public bool IsBlack { get; init; }

        public byte Price { get; init; } = 5;
        public bool IsCanJump { get; set; } = false;
        public Point Position { get; set; }
        public bool isFirstMove = true;
        public bool IsMy { get; init; }

        public Roock(bool isBlask, Point position, bool isMy)
        {
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
        }

        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            King king = FindMyKing(matrix);
            for (int i = Position.Y + 1; i < 8; i++)
            {
                if (matrix[i, Position.X].ChessType != CMT.None && (i == to.Y && Position.X == to.X))
                {
                    isFirstMove = false;
                    return (true);
                }

                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }
            for (int i = Position.Y - 1; i >= 0; i--)
            {
                if (matrix[i, Position.X].ChessType != CMT.None && (i == to.Y && Position.X == to.X))
                {
                    isFirstMove = false;
                    return (true);
                }
                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }

            for (int i = Position.X + 1; i < 8; i++)
            {
                if (matrix[Position.Y, i].ChessType != CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    isFirstMove = false;
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            for (int i = Position.X - 1; i >= 0; i--)
            {
                if (matrix[Position.Y, i].ChessType != CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    isFirstMove = false;
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
                    isFirstMove = false;
                    return (true);
                }
                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }
            for (int i = Position.Y - 1; i >= 0; i--)
            {
                if (matrix[i, Position.X].ChessType == CMT.None && (i == to.Y && Position.X == to.X))
                {
                    isFirstMove = false;
                    return (true);
                }
                if (matrix[i, Position.X].ChessType != CMT.None && !(i == to.Y && Position.X == to.X))
                    break;
            }

            for (int i = Position.X + 1; i < 8; i++)
            {
                if (matrix[Position.Y, i].ChessType == CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    isFirstMove = false;
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            for (int i = Position.X - 1; i >= 0; i--)
            {
                if (matrix[Position.Y, i].ChessType == CMT.None && (Position.Y == to.Y && i == to.X))
                {
                    isFirstMove = false;
                    return (true);
                }
                if (matrix[Position.Y, i].ChessType != CMT.None && !(Position.Y == to.Y && i == to.X))
                    break;
            }
            return (false);
        }
    }
}
