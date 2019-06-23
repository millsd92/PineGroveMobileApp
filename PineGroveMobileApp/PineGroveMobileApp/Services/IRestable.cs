using System.Threading.Tasks;
using PineGroveMobileApp.Models;
using Refit;

namespace PineGroveMobileApp.Services
{
    public interface IRestable
    {
        [Get("/api/users")]
        Task<User[]> GetUsers();

        [Get("/api/users/{username}")]
        Task<User> GetUser(string username);

        [Get("/api/users/GetNames?firstName={firstName}&lastName={lastName}")]
        Task<User[]> GetUsersByName(string firstName, string lastName);
    }
}
