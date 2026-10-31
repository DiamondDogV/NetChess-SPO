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
    }
}
