using Chess.Domain.Interfaces;
using System.Drawing;
using System.Net.NetworkInformation;
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
        public bool IsMy { get; set; }

        public bool IsFirstMove { get; set; }

        public Pawn(bool isBlack, Point position, bool isMy, bool isFirstMove)
        {
            IsFirstMove = isFirstMove;
            IsBlack = isBlack;
            Position = position;
            IsMy = isMy;
        }

        public bool CanMove(in IChess[,] matrix, Point to)
        {
            int direction = IsBlack ? -1 : 1;

            King king = FindMyKing(matrix);
            if (IsFirstMove)
            {
                if (((Position.Y - 2* direction == to.Y) && matrix[Position.Y - 2* direction, Position.X].ChessType == CMT.None) && matrix[Position.Y - direction, Position.X].ChessType == CMT.None
                    && Position.X == to.X)
                {
                    IsFirstMove = false;
                    return true;
                }
            }
            if ((Position.Y - direction == to.Y) && Position.X == to.X)
            {
                IsFirstMove = false;
                return true;
            }
            return false;
        }

        public void ShowMoves(in IChess[,] matrix, out MovesType[,] movesMatrix)
        {
            movesMatrix = new MovesType[8, 8];
            int direction = IsBlack ? 1 : -1;
            int nextY = Position.Y + direction;

            if (nextY >= 0 && nextY <= 7)
            {
                if (matrix[nextY, Position.X].ChessType == CMT.None)
                {
                    movesMatrix[nextY, Position.X] = MovesType.Move;

                    if (IsFirstMove && Position.Y + (2 * direction) >= 0 && Position.Y + (2 * direction) <= 7)
                    {
                        if (matrix[Position.Y + (2 * direction), Position.X].ChessType == CMT.None)
                        {
                            movesMatrix[Position.Y + (2 * direction), Position.X] = MovesType.Move;
                        }
                    }
                }

                if (Position.X - 1 >= 0 && matrix[nextY, Position.X - 1].ChessType != CMT.None && matrix[nextY, Position.X - 1].IsBlack != IsBlack)
                {
                    movesMatrix[nextY, Position.X - 1] = MovesType.Beat;
                }
                if (Position.X + 1 <= 7 && matrix[nextY, Position.X + 1].ChessType != CMT.None && matrix[nextY, Position.X + 1].IsBlack != IsBlack)
                {
                    movesMatrix[nextY, Position.X + 1] = MovesType.Beat;
                }
            }
        }

        public bool CanBeat(in IChess[,] matrix, Point to)
        {
            int direction = IsBlack ? 1 : -1;
            if (to.Y == Position.Y + direction && (to.X == Position.X - 1 || to.X == Position.X + 1))
            {
                if (matrix[to.Y, to.X].ChessType != CMT.None && matrix[to.Y, to.X].IsBlack != IsBlack)
                    return true;
            }
            return false;
        }
    }
}