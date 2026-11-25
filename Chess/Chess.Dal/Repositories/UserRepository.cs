using Chess.Dal.Interfaces;
using Chess.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chess.Dal.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<ApplicationDBContext> _factory;
        public UserRepository(IDbContextFactory<ApplicationDBContext> factory)
            => _factory = factory;

        private ApplicationDBContext Ctx() => _factory.CreateDbContext();

        public async Task<bool> Create(User entity)
        {
            await using var db = Ctx();
            await db.Users.AddAsync(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(User entity)
        {
            await using var db = Ctx();
            db.Users.Remove(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<User?> GetByEmail(string email)
        {
            await using var db = Ctx();
            return await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByEmailAndPass(string email, string pass)
        {
            await using var db = Ctx();
            return await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == pass);
        }

        public async Task<IEnumerable<User>> SelectAll()
        {
            await using var db = Ctx();
            return await db.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> SelectById(long id)
        {
            await using var db = Ctx();
            return await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Update(User entity)
        {
            await using var db = Ctx();
            db.Users.Update(entity);
            return await db.SaveChangesAsync() > 0;
        }
    }
}