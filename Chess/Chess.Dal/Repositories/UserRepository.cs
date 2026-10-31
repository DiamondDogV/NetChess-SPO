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
    public class UserRepository : IUserRepository
    {
        ApplicationDBContext dBContext;
        public UserRepository(ApplicationDBContext dBContext) { this.dBContext = dBContext; }

        public async Task<bool> Create(User entity)
        {
            await dBContext.Users.AddAsync(entity);
            await dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(User entity)
        {
            dBContext.Users.Remove(entity);
            await dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetByEmail(string email) => await dBContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User> GetByEmailAndPass(string email, string pass) => await dBContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == pass);

        public async Task<IEnumerable<User>> SelectAll() => dBContext.Users;

        public async Task<User> SelectById(long id) => await dBContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> Update(User entity)
        {
            dBContext.Users.Update(entity);
            await dBContext.SaveChangesAsync();
            return true;
        }
    }
}
