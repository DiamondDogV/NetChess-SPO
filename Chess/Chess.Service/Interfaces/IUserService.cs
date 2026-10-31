using Chess.Domain.Entities;
using Chess.Domain.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Service.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        public Task<IBaseResponce<User>> GetByEmailAndPassAsync(string email,string pass);
        public Task<IBaseResponce<User>> GetByEmailAsync(string email);

        public ClaimsIdentity Authentication(User user);
    }
}
