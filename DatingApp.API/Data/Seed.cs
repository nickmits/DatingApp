using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext Db;
        public Seed(DataContext cxt)
        {
            Db = cxt;
        }

        public void SeedUsers()
        {
           var userData = File.ReadAllText("Data/UserSeedData.json");
           var users = JsonConvert.DeserializeObject<List<User>>(userData);
           foreach(var user in users)
           {
               CreatePasswordHash("password", out byte[] passwordHash, out byte[] passwordSalt);

               user.PasswordHash = passwordHash;
               user.PasswordSalt = passwordSalt;
               user.Username = user.Username.ToLower();

               Db.Users.Add(user);
           }

           Db.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}