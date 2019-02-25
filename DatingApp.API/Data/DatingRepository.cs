using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext Db;
        public DatingRepository(DataContext cntxt)
        {
            Db = cntxt;
        }
        public void Add<T>(T entity) where T : class
        {
            Db.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            Db.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            return await Db.Users.Include(p => p.Photos).
            FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await Db.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await Db.SaveChangesAsync() > 0;
        }
    }
}