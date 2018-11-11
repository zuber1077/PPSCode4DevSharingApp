using System.Collections.Generic;
using Newtonsoft.Json;
using PpsCode.API.Models;

// Generate seed based UserSeedData.json file and loop throw all user name along with password to be hashed and saved it to db

namespace PpsCode.API.Data
{
  public class Seed
  {
    private readonly DataContext _context;
    public Seed(DataContext context)
    {
      _context = context;

    }

    public void SeedUsers()
    {
        // go out json file and read text from it
        var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
        // list of user obj 
        var users = JsonConvert.DeserializeObject<List<User>>(userData);

        foreach (var user in users)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash("password", out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Username = user.Username.ToLower();

            _context.Users.Add(user);
        }

        _context.SaveChanges();
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      //   hash password
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }
  }
}