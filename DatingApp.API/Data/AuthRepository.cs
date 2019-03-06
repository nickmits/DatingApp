using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext db;
        public AuthRepository(DataContext ctx)
        {
            db = ctx;
        }
        public async Task<User> Login(string username, string password)
        {
            try
            {
                var user = await db.Users.Include(p => p.Photos).FirstAsync(usr => usr.Username == username);
                VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
                return user;
            }
            catch
            {
                return null;
            }
        }



        public async Task<User> Register(User user, string password)
        {
            CreatePasswordHash(password, out byte[] PasswordHash, out byte[] PasswordSalt);
            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private void VerifyPasswordHash(string password, byte[] passwordHash, byte[] saltHash)
        {
            using (var hmac = new HMACSHA512(saltHash))
            {
                byte[] ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != ComputeHash[i])
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }


        public async Task<bool> UserExists(string username)
        {
            return await db.Users.AnyAsync(usr => usr.Username == username);
        }
    }
}