using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PpsCode.API.Models;

namespace PpsCode.API.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext _context;
    public AuthRepository(DataContext context)
    {
      _context = context;

    }
 /*  Login Start */
    public async Task<User> Login(string username, string password)
    {
    //   (x) to tell which user we lucking for
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

        if (user == null)
        {
            return null;
        }

        //  compute (( compare the password to z hash 
        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }

        // successful
        return user;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        // using the key )) if the hash match = true
      using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
      {
        var ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        // loop each element array to compare each element
        for (int i = 0; i < ComputedHash.Length; i++)
        {
            if (ComputedHash[i] != passwordHash[i])
            {
                return false;
            }
        }
      }
      //   if match
      return true;
    }

    /*  Login end */


    /*  Register Start */

    public async Task<User> Register(User user, string password)
    {
      byte[] passwordHash, passwordSalt;
        // reference useing <out> if updated also updated   
      CreatePasswordHash(password, out passwordHash, out passwordSalt);

      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync(); // save changes back to db

      return user;
    }

    // Genereate Method
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
    //   hash password
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

/*  Register end */


/*  User Exists start */

    public async Task<bool> UserExists(string username)
    {
      if (await _context.Users.AnyAsync(x => x.Username == username))
      {
          return true;
      }
      return false;
    }
/*  User Exists end */
  }
}