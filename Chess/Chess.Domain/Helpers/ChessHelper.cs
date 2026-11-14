using Chess.Domain.Entities;
using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;

namespace Chess.Domain.Helpers
{
    public static class ChessHelper
    {
        public static King FindMyKing(in IChess[,] matrix)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (matrix[i, j].IsMy && matrix[i, j].ChessType == CMT.King)
                    {
                        return matrix[i, j] as King;
                    }
                }
            return null;
        }

        static IChess GetChess(Type type, bool isBlack, Point position, bool isMy, bool isFirstMove)
        {
            if (type == typeof(Cell)) return new Cell();
            else if (type == typeof(Ferz)) return new Ferz(isBlack, position, isMy, isFirstMove);
            else if (type == typeof(King)) return new King(isBlack, position, isMy, isFirstMove);
            else if (type == typeof(Knight)) return new Knight(isBlack, position, isMy, isFirstMove);
            else if (type == typeof(Pawn)) return new Pawn(isBlack, position, isMy, isFirstMove);
            else if (type == typeof(Roock)) return new Roock(isBlack, position, isMy, isFirstMove);
            else if (type == typeof(Slon)) return new Slon(isBlack, position, isMy, isFirstMove);
            return new Cell();
        }

        public static bool isMat(in IChess[,] matrix, Point lastbeat)
        {
            King myKing = ChessHelper.FindMyKing(matrix);
            if (myKing == null) return false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (matrix[i, j] == myKing)
                    {
                        myKing.Position = new Point(j, i);
                        break;
                    }
                }
            }

            bool isKingUnderCheckNow = !myKing.CheckStillExist(matrix, new Point(-1, -1));

            for (int fromY = 0; fromY < 8; fromY++)
            {
                for (int fromX = 0; fromX < 8; fromX++)
                {
                    IChess piece = matrix[fromY, fromX];

                    if (piece.ChessType != CMT.None && piece.IsMy)
                    {
                        piece.Position = new Point(fromX, fromY);

                        for (int toY = 0; toY < 8; toY++)
                        {
                            for (int toX = 0; toX < 8; toX++)
                            {
                                Point targetPoint = new Point(toX, toY);
                                IChess targetPiece = matrix[toY, toX];

                                bool canMove = (targetPiece.ChessType == CMT.None) && piece.CanMove(matrix, targetPoint);
                                bool canBeat = (!targetPiece.IsMy && targetPiece.ChessType != CMT.None) && piece.CanBeat(in matrix, targetPoint);

                                if (canMove || canBeat)
                                {
                                    IChess[,] simulationBoard = new IChess[8, 8];
                                    ChessHelper.CopyMatrix(matrix, ref simulationBoard);

                                    for (int y = 0; y < 8; y++)
                                        for (int x = 0; x < 8; x++)
                                            if (simulationBoard[y, x].ChessType != CMT.None)
                                                simulationBoard[y, x].Position = new Point(x, y);

                                    IChess copyOfMyFigure = simulationBoard[fromY, fromX];
                                    simulationBoard[toY, toX] = copyOfMyFigure;
                                    simulationBoard[toY, toX].Position = targetPoint;
                                    simulationBoard[fromY, fromX] = new Cell();

                                    King virtualKing = FindMyKing(simulationBoard);
                                    if (virtualKing != null)
                                    {
                                        if (virtualKing.CheckStillExist(simulationBoard, new Point(-1, -1)))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return isKingUnderCheckNow;
        }


        public static IChess[,] CreateStartBoard()
        {
            var board = new IChess[8, 8];

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    board[y, x] = new Cell();

            CMT[] backRow = { CMT.Roock, CMT.Knight, CMT.Slon, CMT.Ferz, CMT.King, CMT.Slon, CMT.Knight, CMT.Roock };

            for (int x = 0; x < 8; x++)
            {
                board[0, x] = CreatePiece(backRow[x], true, new Point(x, 0), false, true);
                board[1, x] = CreatePiece(CMT.Pawn, true, new Point(x, 1), false, true);
            }

            for (int x = 0; x < 8; x++)
            {
                board[6, x] = CreatePiece(CMT.Pawn, false, new Point(x, 6), true, true);
                board[7, x] = CreatePiece(backRow[x], false, new Point(x, 7), true, true);
            }

            return board;
        }

        public static void CopyMatrix(in IChess[,] matrix, ref IChess[,] matrix2)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    matrix2[i, j] = GetChess(matrix[i, j].GetType(), matrix[i, j].IsBlack, matrix[i, j].Position, matrix[i, j].IsMy, matrix[i, j].IsFirstMove);
                }
            }
        }

        public static bool EmulateMoves(in IChess[,] matrix, IChess this_, Point Position, Point to, King king)
        {
            if (king == null)
            {
                king = FindMyKing(in matrix);
            }
            IChess[,] matrix2 = new IChess[8, 8];
            CopyMatrix(matrix, ref matrix2);
            matrix2[to.Y, to.X] = this_;
            matrix2[Position.Y, Position.X] = new Cell();
            if (this_ == king)
            {
                return king.CheckStillExist(matrix2, to);
            }
            else
            {
                return king.CheckStillExist(matrix2, new Point(-1, -1));
            }
        }

        public static IChess CreatePiece(CMT type, bool isBlack, Point position, bool isMy, bool isFirstMove)
        {
            return type switch
            {
                CMT.None => new Cell(),
                CMT.Ferz => new Ferz(isBlack, position, isMy, isFirstMove),
                CMT.King => new King(isBlack, position, isMy, isFirstMove),
                CMT.Knight => new Knight(isBlack, position, isMy, isFirstMove),
                CMT.Pawn => new Pawn(isBlack, position, isMy, isFirstMove),
                CMT.Roock => new Roock(isBlack, position, isMy, isFirstMove),
                CMT.Slon => new Slon(isBlack, position, isMy, isFirstMove),
                _ => new Cell()
            };
        }
        
        public static string Serialize(IChess[,] board)
        {
            var sb = new StringBuilder(64 * 4);
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = board[y, x];
                    if (piece.ChessType == CMT.None)
                    {
                        sb.Append("____");
                    }
                    else
                    {
                        string color = piece.IsBlack ? "b" : "w";
                        string type = piece.ChessType switch
                        {
                            CMT.Pawn => "P",
                            CMT.Knight => "N",
                            CMT.Slon => "B",
                            CMT.Roock => "R",
                            CMT.Ferz => "Q",
                            CMT.King => "K",
                            _ => "X"
                        };
                        string isMy = piece.IsMy ? "1" : "0";
                        string isFirstMove = piece.IsFirstMove ? "1" : "0";
                        sb.Append(color + type + isMy + isFirstMove);
                    }
                }
            }
            return sb.ToString();
        }

        public static IChess[,] Deserialize(string data)
        {
            var board = new IChess[8, 8];
            if (string.IsNullOrEmpty(data) || data.Length < 64 * 4)
            {
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        board[y, x] = new Cell();
                return board;
            }

            int index = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    string chunk = data.Substring(index, 4);
                    index += 4;

                    if (chunk == "____")
                    {
                        board[y, x] = new Cell();
                    }
                    else
                    {
                        bool isBlack = chunk[0] == 'b';
                        bool isMy = chunk[2] == '1';
                        bool isFirstMove = chunk[3] == '1';

                        CMT type = chunk[1] switch
                        {
                            'P' => CMT.Pawn,
                            'N' => CMT.Knight,
                            'B' => CMT.Slon,
                            'R' => CMT.Roock,
                            'Q' => CMT.Ferz,
                            'K' => CMT.King,
                            _ => CMT.None
                        };

                        board[y, x] = ChessHelper.CreatePiece(type, isBlack, new System.Drawing.Point(x, y), isMy, isFirstMove);
                    }
                }
            }
            return board;
        }
    }
}
