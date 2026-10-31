using Chess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Dal.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task<User> GetByEmailAndPass(string email, string pass);
        public Task<User> GetByEmail(string email);
    }
}
