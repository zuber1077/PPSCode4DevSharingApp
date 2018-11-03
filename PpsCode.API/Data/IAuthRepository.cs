using System.Threading.Tasks;
using PpsCode.API.Models;

namespace PpsCode.API.Data
{
    public interface IAuthRepository
    {
        //  register the users
        Task<User> Register(User user, string password);
        // method for the user to log in to API
        Task<User> Login(string username, string password);
        // method to check if a user exists
        Task<bool> UserExists(string username);
    }
}
