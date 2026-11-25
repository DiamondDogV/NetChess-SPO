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
            bool isMove, isBeat;

            if (board[req.ToY, req.ToX].ChessType == CMT.None)
                isMove = selected.CanMove(board, target);
            else
                isMove = false;

            if (!isMove)
                isBeat = selected.CanBeat(in board, target);
            else
                isBeat = false;

            if (!isMove && !isBeat)
                return Task.FromResult(new MoveOutcome { Success = false, Error = "Недопустимый ход" });

            var king = ChessHelper.FindMyKing(board);
            if (!ChessHelper.EmulateMoves(board, selected, new Point(req.FromX, req.FromY), target, king))
                return Task.FromResult(new MoveOutcome { Success = false, Error = "Король под шахом" });

            if (selected.ChessType == CMT.King && Math.Abs(req.ToX - req.FromX) == 2)
            {
                int rookFromX = req.ToX > req.FromX ? 7 : 0;
                int rookToX = req.ToX > req.FromX ? req.ToX - 1 : req.ToX + 1;
                var rook = board[req.FromY, rookFromX];
                board[req.FromY, rookToX] = rook;
                board[req.FromY, rookFromX] = new Cell();
                rook.Position = new Point(rookToX, req.FromY);
                rook.IsFirstMove = false;
            }

            selected.Position = target;
            board[req.ToY, req.ToX] = selected;
            board[req.FromY, req.FromX] = new Cell();
            selected.IsFirstMove = false;

            if (selected.ChessType == CMT.Pawn && (req.ToY == 0 || req.ToY == 7))
            {
                var promoted = new Ferz(selected.IsBlack, target, selected.IsMy, false);
                board[req.ToY, req.ToX] = promoted;
            }

            room.BoardState = ChessHelper.Serialize(board);
            room.IsWhiteTurn = !room.IsWhiteTurn;

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