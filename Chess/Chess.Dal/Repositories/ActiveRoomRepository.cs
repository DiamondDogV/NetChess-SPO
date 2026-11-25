using Chess.Dal.Interfaces;
using Chess.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chess.Dal.Repositories
{
    public class ActiveRoomRepository : IActiveRoomRepository
    {
        private readonly IDbContextFactory<ApplicationDBContext> _factory;
        public ActiveRoomRepository(IDbContextFactory<ApplicationDBContext> factory)
            => _factory = factory;

        private ApplicationDBContext Ctx() => _factory.CreateDbContext();

        public async Task<bool> Create(ActiveRoom entity)
        {
            await using var db = Ctx();
            await db.ActiveRooms.AddAsync(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(ActiveRoom entity)
        {
            await using var db = Ctx();
            db.ActiveRooms.Remove(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ActiveRoom>> SelectAll()
        {
            await using var db = Ctx();
            return await db.ActiveRooms.AsNoTracking().ToListAsync();
        }

        public async Task<ActiveRoom?> SelectById(long id)
        {
            await using var db = Ctx();
            return await db.ActiveRooms.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Update(ActiveRoom entity)
        {
            await using var db = Ctx();
            db.ActiveRooms.Update(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<ActiveRoom?> GetByToken(string token)
        {
            await using var db = Ctx();
            return await db.ActiveRooms.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<bool> TouchHeartbeat(string token)
        {
            await using var db = Ctx();
            var room = await db.ActiveRooms.FirstOrDefaultAsync(x => x.Token == token);
            if (room == null) return false;
            room.LastHeartbeat = DateTime.UtcNow;
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<int> RemoveStale(DateTime olderThan)
        {
            await using var db = Ctx();
            var stale = await db.ActiveRooms
                .Where(x => x.LastHeartbeat < olderThan)
                .ToListAsync();
            if (stale.Count == 0) return 0;
            db.ActiveRooms.RemoveRange(stale);
            await db.SaveChangesAsync();
            return stale.Count;
        }

        public async Task<bool> DeleteByToken(string token)
        {
            await using var db = Ctx();
            var room = await db.ActiveRooms.FirstOrDefaultAsync(x => x.Token == token);
            if (room == null) return false;
            db.ActiveRooms.Remove(room);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<List<ActiveRoom>> GetOpenRooms()
        {
            await using var db = Ctx();
            return await db.ActiveRooms
                .Where(r => r.BlackConnectionId == null && !r.IsMate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}