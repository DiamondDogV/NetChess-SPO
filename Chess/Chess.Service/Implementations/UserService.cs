using Chess.Dal.Interfaces;
using Chess.Dal.Repositories;
using Chess.Domain.Entities;
using Chess.Domain.Responces;
using Chess.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Service.Implementations
{
    public class UserService : IUserService
    {
        IUserRepository rep;
        public UserService(IUserRepository rep) { this.rep = rep; }

        public ClaimsIdentity Authentication(User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType,user.Email)
            };
            return new ClaimsIdentity(claim, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        public async Task<IBaseResponce<bool>> CreateAsync(User entity)
        {
            try
            {
                if (entity != null)
                {
                    User user = await rep.GetByEmail(entity.Email);
                    if (user == null)
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
                    else
                    {
                        return new BaseResponce<bool>()
                        {
                            Data = false,
                            Code = Domain.Enums.Enums.ResponceCodes.UserExist,
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

        public async Task<IBaseResponce<IEnumerable<User>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IBaseResponce<User>> GetByEmailAsync(string email)
        {
            try
            {
                if (email != null)
                {
                    User user = await rep.GetByEmail(email);
                    if (user != null)
                    {
                        return new BaseResponce<User>()
                        {
                            Data = user,
                            Code = Domain.Enums.Enums.ResponceCodes.Ok,
                        };
                    }
                    else
                    {
                        return new BaseResponce<User>()
                        {
                            Data = null,
                            Code = Domain.Enums.Enums.ResponceCodes.UserDoesntExist,
                        };
                    }
                }
                return new BaseResponce<User>()
                {
                    Data = null,
                    Code = Domain.Enums.Enums.ResponceCodes.InvalidParameter,
                };
            }
            catch (Exception)
            {
                return new BaseResponce<User>()
                {
                    Data = null,
                    Code = Domain.Enums.Enums.ResponceCodes.UnCatchedError,
                };
            }
        }

        public async Task<IBaseResponce<User>> GetByEmailAndPassAsync(string email, string pass)
        {
            try
            {
                if (email != null)
                {
                    User user = await rep.GetByEmailAndPass(email,pass);
                    if (user != null)
                    {
                        return new BaseResponce<User>()
                        {
                            Data = user,
                            Code = Domain.Enums.Enums.ResponceCodes.Ok,
                        };
                    }
                    else
                    {
                        return new BaseResponce<User>()
                        {
                            Data = null,
                            Code = Domain.Enums.Enums.ResponceCodes.UserDoesntExist,
                        };
                    }
                }
                return new BaseResponce<User>()
                {
                    Data = null,
                    Code = Domain.Enums.Enums.ResponceCodes.InvalidParameter,
                };
            }
            catch (Exception)
            {
                return new BaseResponce<User>()
                {
                    Data = null,
                    Code = Domain.Enums.Enums.ResponceCodes.UnCatchedError,
                };
            }
        }

        public async Task<IBaseResponce<User>> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IBaseResponce<bool>> RemoveAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IBaseResponce<bool>> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
