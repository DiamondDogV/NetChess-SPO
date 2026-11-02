using Chess.Domain.Entities;
using Chess.Domain.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Service.Interfaces
{
    public interface IActiveRoomService : IBaseService<ActiveRoom>
    {
        Task<IBaseResponce<ActiveRoom>> CreateForTabAsync(ActiveRoom entity);
        Task<IBaseResponce<ActiveRoom>> GetByTokenAsync(string token);
        Task<IBaseResponce<bool>> HeartbeatAsync(string token);
        Task<IBaseResponce<bool>> CloseAsync(string token);
        Task<int> CleanupStaleAsync(TimeSpan ttl);
    }
}
