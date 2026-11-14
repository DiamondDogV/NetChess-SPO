using Chess.Dal.Interfaces;
using Chess.Domain.Entities;
using Chess.Domain.Responces;
using Chess.Service.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;

namespace Chess.Service.Implementations
{
    public class ActiveRoomService : IActiveRoomService
    {
        IActiveRoomRepository rep;
        public ActiveRoomService(IActiveRoomRepository rep) { this.rep = rep; }

        public async Task<IBaseResponce<bool>> CreateAsync(ActiveRoom entity)
        {
            try
            {
                if (entity != null)
                {
                    bool res = await rep.Create(entity);
                    if (res)
                    {
                        return new BaseResponce<bool>()
                        {
                            Data = res,
                            Code = Domain.Enums.Enums.ResponceCodes.Ok,
                        };
                    }
                    else
                    {
                        return new BaseResponce<bool>()
                        {
                            Data = res,
                            Code = Domain.Enums.Enums.ResponceCodes.BoolError,
                        };
                    }
                }
                return new BaseResponce<bool>()
                {
                    Data = false,
                    Code = Domain.Enums.Enums.ResponceCodes.InvalidParameter,
                };
            }
            catch (Exception)
            {
                return new BaseResponce<bool>()
                {
                    Data = false,
                    Code = Domain.Enums.Enums.ResponceCodes.UnCatchedError,
                };
            }
        }

        public async Task<IBaseResponce<IEnumerable<ActiveRoom>>> GetAllAsync()
        {
            try
            {
                IEnumerable<ActiveRoom> res = await rep.SelectAll();
                if (res.Count()>0)
                {
                    return new BaseResponce<IEnumerable<ActiveRoom>> ()
                    {
                        Data = res.ToList(),
                        Code = Domain.Enums.Enums.ResponceCodes.Ok,
                    };
                }
                else
                {
                    return new BaseResponce<IEnumerable<ActiveRoom>> ()
                    {
                        Data = null,
                        Code = Domain.Enums.Enums.ResponceCodes.NullTable,
                    };
                }

            }
            catch (Exception)
            {
                return new BaseResponce<IEnumerable < ActiveRoom >> ()
                {
                    Data = null,
                    Code = Domain.Enums.Enums.ResponceCodes.UnCatchedError,
                };
            }
        }

        public async Task<IBaseResponce<ActiveRoom>> GetByIdAsync(long id)
        {
            try
            {
                var room = await rep.SelectById(id);
                if (room != null)
                {
                    return new BaseResponce<ActiveRoom>()
                    {
                        Data = room,
                        Code = ResponceCodes.Ok,
                    };
                }
                return new BaseResponce<ActiveRoom>()
                {
                    Data = null,
                    Code = ResponceCodes.NullTable,
                };
            }
            catch (Exception)
            {
                return new BaseResponce<ActiveRoom>()
                {
                    Data = null,
                    Code = ResponceCodes.UnCatchedError,
                };
            }
        }

        public async Task<IBaseResponce<bool>> RemoveAsync(ActiveRoom entity)
        {
            try
            {
                if (entity != null)
                {
                    bool res = await rep.Delete(entity);
                    return new BaseResponce<bool>()
                    {
                        Data = res,
                        Code = res ? ResponceCodes.Ok : ResponceCodes.BoolError,
                    };
                }
                return new BaseResponce<bool>()
                {
                    Data = false,
                    Code = ResponceCodes.InvalidParameter,
                };
            }
            catch (Exception)
            {
                return new BaseResponce<bool>()
                {
                    Data = false,
                    Code = ResponceCodes.UnCatchedError,
                };
            }
        }

        public async Task<IBaseResponce<bool>> UpdateAsync(ActiveRoom entity)
        {
            try
            {
                if (entity != null)
                {
                    bool res = await rep.Update(entity);
                    return new BaseResponce<bool>()
                    {
                        Data = res,
                        Code = res ? ResponceCodes.Ok : ResponceCodes.BoolError,
                    };
                }
                return new BaseResponce<bool>()
                {
                    Data = false,
                    Code = ResponceCodes.InvalidParameter,
                };
            }
            catch (Exception)
            {
                return new BaseResponce<bool>()
                {
                    Data = false,
                    Code = ResponceCodes.UnCatchedError,
                };
            }
        }



        public async Task<IBaseResponce<ActiveRoom>> CreateForTabAsync(ActiveRoom entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.Token))
                return new BaseResponce<ActiveRoom> { Data = null, Code = ResponceCodes.InvalidParameter };

            entity.CreatedAt = DateTime.UtcNow;
            entity.LastHeartbeat = DateTime.UtcNow;

            var ok = await rep.Create(entity);
            return new BaseResponce<ActiveRoom>
            {
                Data = ok ? entity : null,
                Code = ok ? ResponceCodes.Ok : ResponceCodes.BoolError
            };
        }

        public async Task<IBaseResponce<ActiveRoom>> GetByTokenAsync(string token)
        {
            var room = await rep.GetByToken(token);
            return new BaseResponce<ActiveRoom>
            {
                Data = room,
                Code = room != null ? ResponceCodes.Ok : ResponceCodes.NullTable
            };
        }

        public async Task<IBaseResponce<bool>> HeartbeatAsync(string token)
        {
            var ok = await rep.TouchHeartbeat(token);
            return new BaseResponce<bool> { Data = ok, Code = ok ? ResponceCodes.Ok : ResponceCodes.BoolError };
        }

        public async Task<IBaseResponce<bool>> CloseAsync(string token)
        {
            var ok = await rep.DeleteByToken(token);
            return new BaseResponce<bool> { Data = ok, Code = ok ? ResponceCodes.Ok : ResponceCodes.BoolError };
        }

        public async Task<int> CleanupStaleAsync(TimeSpan ttl)
            => await rep.RemoveStale(DateTime.UtcNow - ttl);
    }
}
