using Refit;
using System.Threading;
using System.Threading.Tasks;
using PineGroveMobileApp.Models;

namespace PineGroveMobileApp.Services
{
    public class RestClient
    {
        private readonly IRestable _client;

        public RestClient()
        {
            
            RefitSettings settings = new RefitSettings() { ContentSerializer = new JsonContentSerializer() };
            _client = RestService.For<IRestable>("https://36vhnw37q6.execute-api.us-east-2.amazonaws.com/Prod", settings);
        }

        public async Task<User[]> GetUsers()
        {
            return await _client.GetUsers();
        }

        public async Task<User> GetUser(string username, CancellationToken token)
        {
            return await _client.GetUser(username, token);
        }

        public async Task<User[]> GetUsersByName(string firstName, string lastName, CancellationToken token)
        {
            return await _client.GetUsersByName(firstName, lastName, token);
        }

        public async Task<AnnouncementRequest> CreateAnnouncement([Body] AnnouncementRequest announcement, CancellationToken token)
        {
            return await _client.CreateAnnouncement(announcement, token);
        }

        public async Task<AnnouncementRequest> GetAnnouncement(int announcementId)
        {
            return await _client.GetAnnouncement(announcementId);
        }

        public async Task<Event[]> GetEvents()
        {
            return await _client.GetEvents();
        }

        public async Task<EventRegistration> CreateRegistration([Body] EventRegistration registration, CancellationToken token)
        {
            return await _client.CreateRegistration(registration, token);
        }

        public async Task<Event> UpdateEvent(int eventId, [Body] Event @event, CancellationToken token)
        {
            return await _client.UpdateEvent(eventId, @event, token);
        }

        public async Task<User> CreateUser([Body] User user, CancellationToken token)
        {
            return await _client.CreateUser(user, token);
        }

        public async Task<User> UpdateUser(int userId, [Body] User user, CancellationToken token)
        {
            return await _client.UpdateUser(userId, user, token);
        }
    }
}
