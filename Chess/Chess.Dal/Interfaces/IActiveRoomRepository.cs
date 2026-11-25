using Chess.Domain.Entities;

namespace Chess.Dal.Interfaces
{
    public interface IActiveRoomRepository : IBaseRepository<ActiveRoom>
    {
        Task<ActiveRoom?> GetByToken(string token);
        Task<bool> TouchHeartbeat(string token);
        Task<int> RemoveStale(DateTime olderThan);
        Task<bool> DeleteByToken(string token);
        Task<List<ActiveRoom>> GetOpenRooms();
    }
}