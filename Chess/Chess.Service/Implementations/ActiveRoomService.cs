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
            throw new NotImplementedException();
        }

        public async Task<IBaseResponce<bool>> RemoveAsync(ActiveRoom entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IBaseResponce<bool>> UpdateAsync(ActiveRoom entity)
        {
            throw new NotImplementedException();
        }
    }
}
