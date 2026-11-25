using Chess.Domain.DTO;
using Chess.Domain.Entities;
using Chess.Domain.Helpers;
using Chess.Domain.Interfaces;
using Chess.Service.Interfaces;
using System.Drawing;
using static Chess.Domain.Enums.Enums;

namespace Chess.Service.Implementations
{
    public class GameService : IGameService
    {
        public Task<SelectOutcome> SelectPieceAsync(ActiveRoom room, bool playerIsWhite, MoveRequest req)
        {
            var board = ChessHelper.Deserialize(room.BoardState);
            ApplyIsMy(board, playerIsWhite);

            var piece = board[req.FromY, req.FromX];
            if (piece.ChessType == CMT.None)
                return Task.FromResult(new SelectOutcome { Success = false, Error = "Пустая клетка" });
            if (piece.IsBlack == playerIsWhite)
                return Task.FromResult(new SelectOutcome { Success = false, Error = "Это не ваша фигура!" });

            piece.ShowMoves(in board, out MovesType[,] movesMatrix);
            return Task.FromResult(new SelectOutcome
            {
                Success = true,
                Moves = ConvertMovesToJson(movesMatrix)
            });
        }

        public Task<MoveOutcome> MakeMoveAsync(ActiveRoom room, bool playerIsWhite, MoveRequest req)
        {
            var board = ChessHelper.Deserialize(room.BoardState);
            ApplyIsMy(board, playerIsWhite);

            var selected = board[req.FromY, req.FromX];
            if (selected.ChessType == CMT.None || selected.IsBlack == playerIsWhite)
                return Task.FromResult(new MoveOutcome { Success = false, Error = "Неверная фигура" });

            var target = new Point(req.ToX, req.ToY);
            bool isMove = false;
            bool isBeat = false;
            bool isCastling = false;

            // === ПРОВЕРКА РОКИРОВКИ ===
            if (selected is King king)
            {
                // Если король пытается пойти на 2 клетки в сторону
                if (Math.Abs(req.ToX - req.FromX) == 2 && req.ToY == req.FromY)
                {
                    if (king.CanMakeRokir(board, target))
                    {
                        isMove = true;
                        isCastling = true;
                    }
                }
                // Если игрок кликнул на ладью (h1/a1), а не на пустую клетку
                else if (Math.Abs(req.ToX - req.FromX) > 2) // Клик на ладью
                {
                    Point rookPoint = new Point(req.ToX, req.ToY);
                    // Проверяем, что там стоит ладья
                    if (board[req.ToY, req.ToX] is Roock)
                    {
                        if (king.CanMakeRokir(board, rookPoint))
                        {
                            isMove = true;
                            isCastling = true;
                            // Меняем target, чтобы он указывал на конечную позицию короля
                            target = new Point(req.ToX > req.FromX ? 6 : 2, req.FromY);
                        }
                    }
                }
            }

            // Если это не рокировка, проверяем обычный ход
            if (!isCastling)
            {
                if (board[req.ToY, req.ToX].ChessType == CMT.None)
                    isMove = selected.CanMove(board, target);
                else
                    isBeat = selected.CanBeat(in board, target);
            }

            if (!isMove && !isBeat)
                return Task.FromResult(new MoveOutcome { Success = false, Error = "Недопустимый ход" });

            // Проверка на шах
            var myKing = ChessHelper.FindMyKing(board);
            if (!ChessHelper.EmulateMoves(board, selected, new Point(req.FromX, req.FromY), target, myKing))
                return Task.FromResult(new MoveOutcome { Success = false, Error = "Король под шахом" });

            // === ПРИМЕНЕНИЕ РОКИРОВКИ ===
            if (isCastling)
            {
                int row = req.FromY;
                int kingToX, rookFromX, rookToX;

                if (req.ToX > req.FromX)
                {
                    kingToX = 6;      // g1 / g8
                    rookFromX = 7;    // h1 / h8
                    rookToX = 5;      // f1 / f8
                }
                else
                {
                    kingToX = 2;      // c1 / c8
                    rookFromX = 0;    // a1 / a8
                    rookToX = 3;      // d1 / d8
                }

                var rook = board[row, rookFromX];
                var kingPiece = board[row, req.FromX];

                // 1. Очищаем старые клетки
                board[row, req.FromX] = new Cell();  // e1
                board[row, rookFromX] = new Cell();  // h1 или a1

                // 2. Ставим короля
                kingPiece.Position = new Point(kingToX, row);
                board[row, kingToX] = kingPiece;
                kingPiece.IsFirstMove = false;

                // 3. Ставим ладью
                rook.Position = new Point(rookToX, row);
                board[row, rookToX] = rook;
                rook.IsFirstMove = false;
            }
            else
            {
                // Обычный ход
                selected.Position = target;
                board[req.ToY, req.ToX] = selected;
                board[req.FromY, req.FromX] = new Cell();
                selected.IsFirstMove = false;

                // Превращение пешки
                if (selected.ChessType == CMT.Pawn && (req.ToY == 0 || req.ToY == 7))
                {
                    board[req.ToY, req.ToX] = new Ferz(selected.IsBlack, target, selected.IsMy, false);
                }
            }

            room.BoardState = ChessHelper.Serialize(board);
            room.IsWhiteTurn = !room.IsWhiteTurn;

            // Мат
            ApplyIsMy(board, !playerIsWhite);
            bool isMate = ChessHelper.isMat(board, target);
            room.IsMate = isMate;

            return Task.FromResult(new MoveOutcome
            {
                Success = true,
                Board = board,
                IsWhiteTurn = room.IsWhiteTurn,
                IsMate = isMate
            });
        }

        private static void ApplyIsMy(IChess[,] board, bool playerIsWhite)
        {
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if (board[y, x].ChessType != CMT.None)
                        board[y, x].IsMy = board[y, x].IsBlack != playerIsWhite;
        }

        private static List<List<int>> ConvertMovesToJson(MovesType[,] moves)
        {
            var result = new List<List<int>>();
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if (moves[y, x] != MovesType.None)
                        result.Add(new List<int> { x, y, (int)moves[y, x] });
            return result;
        }
    }
}