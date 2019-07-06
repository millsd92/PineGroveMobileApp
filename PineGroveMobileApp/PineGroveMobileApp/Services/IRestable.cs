using System.Threading.Tasks;
using PineGroveMobileApp.Models;
using Refit;
using System.Threading;

namespace PineGroveMobileApp.Services
{
    public interface IRestable
    {
        [Get("/api/users")]
        Task<User[]> GetUsers();

        [Get("/api/users/{username}")]
        Task<User> GetUser(string username, CancellationToken token);

        [Get("/api/users/GetNames?firstName={firstName}&lastName={lastName}")]
        Task<User[]> GetUsersByName(string firstName, string lastName, CancellationToken token);

        [Post("/api/announcementrequests")]
        Task<AnnouncementRequest> CreateAnnouncement([Body] AnnouncementRequest announcement, CancellationToken token);

        [Get("/api/AnnouncementRequests/{announcementId}")]
        Task<AnnouncementRequest> GetAnnouncement(int announcementId);

        [Get("/api/Events")]
        Task<Event[]> GetEvents();

        [Post("/api/eventregistrations")]
        Task<EventRegistration> CreateRegistration([Body] EventRegistration registration, CancellationToken token);

        [Put("/api/Events/{eventId}")]
        Task<Event> UpdateEvent(int eventId, [Body] Event @event, CancellationToken token);

        [Post("/api/users")]
        Task<User> CreateUser([Body] User user, CancellationToken token);

        [Put("/api/users/{userId}")]
        Task<User> UpdateUser(int userId, [Body] User user, CancellationToken token);
    }
}
