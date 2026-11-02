using Chess.Dal.Interfaces;
using Chess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Dal.Repositories
{
    public class ActiveRoomRepository : IActiveRoomRepository
    {
        ApplicationDBContext dBContext;
        public ActiveRoomRepository(ApplicationDBContext dBContext) { this.dBContext = dBContext; }

        public async Task<bool> Create(ActiveRoom entity)
        {
            await dBContext.ActiveRooms.AddAsync(entity);
            await dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(ActiveRoom entity)
        {
            dBContext.ActiveRooms.Remove(entity);
            await dBContext.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ActiveRoom>> SelectAll() => dBContext.ActiveRooms;

        public async Task<ActiveRoom> SelectById(long id) => await dBContext.ActiveRooms.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> Update(ActiveRoom entity)
        {
            dBContext.ActiveRooms.Update(entity);
            await dBContext.SaveChangesAsync();
            return true;
        }


        public async Task<ActiveRoom> GetByToken(string token) => await dBContext.ActiveRooms.FirstOrDefaultAsync(x => x.Token == token);

        public async Task<bool> TouchHeartbeat(string token)
        {
            var room = await dBContext.ActiveRooms.FirstOrDefaultAsync(x => x.Token == token);
            if (room == null) return false;
            room.LastHeartbeat = DateTime.UtcNow;
            await dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> RemoveStale(DateTime olderThan)
        {
            var stale = await dBContext.ActiveRooms.Where(x => x.LastHeartbeat < olderThan).ToListAsync();
            if (!stale.Any()) return 0;
            dBContext.ActiveRooms.RemoveRange(stale);
            await dBContext.SaveChangesAsync();
            return stale.Count;
        }

        public async Task<bool> DeleteByToken(string token)
        {
            var room = await dBContext.ActiveRooms.FirstOrDefaultAsync(x => x.Token == token);
            if (room == null) return false;
            dBContext.ActiveRooms.Remove(room);
            await dBContext.SaveChangesAsync();
            return true;
        }
    }
}
