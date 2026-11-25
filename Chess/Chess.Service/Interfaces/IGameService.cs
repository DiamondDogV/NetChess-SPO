using Chess.Domain.DTO;
using Chess.Domain.Entities;
using Chess.Domain.Interfaces;

namespace Chess.Service.Interfaces
{
    public class MoveOutcome
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public IChess[,]? Board { get; set; }
        public bool IsWhiteTurn { get; set; }
        public bool IsMate { get; set; }
    }

    public class SelectOutcome
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public List<List<int>>? Moves { get; set; }
    }

    public interface IGameService
    {
        Task<SelectOutcome> SelectPieceAsync(ActiveRoom room, bool playerIsWhite, MoveRequest req);
        Task<MoveOutcome> MakeMoveAsync(ActiveRoom room, bool playerIsWhite, MoveRequest req);
    }
}