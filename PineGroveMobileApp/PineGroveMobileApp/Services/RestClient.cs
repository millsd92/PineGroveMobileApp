using Refit;
using System.Threading;
using System.Threading.Tasks;
using PineGroveMobileApp.Models;

namespace PineGroveMobileApp.Services
{
    public class RestClient
    {
        // This is the actual client.
        private readonly IRestable _client;
        private static readonly CancellationTokenSource source = new CancellationTokenSource();

        /// <summary>
        /// This is the object that provides implementation of the IRestable interface and configures it.
        /// </summary>
        public RestClient()
        {
            
            RefitSettings settings = new RefitSettings() { ContentSerializer = new JsonContentSerializer() };
            _client = RestService.For<IRestable>("https://36vhnw37q6.execute-api.us-east-2.amazonaws.com/Prod", settings);
            source.CancelAfter((int)App.timeoutTime);
        }

        /// <summary>
        /// This calls the API and returns a single user based on username.
        /// </summary>
        /// <param name="username">The username to be searched for.</param>
        /// <returns>The user who's username matches the string provided.</returns>
        public async Task<User> GetUser(string username)
        {
            return await _client.GetUser(username, source.Token);
        }

        /// <summary>
        /// This calls the API and returns all users who have a given first and last name.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <returns>An array of all of the users who have the given first and last name.</returns>
        public async Task<User[]> GetUsersByName(string firstName, string lastName)
        {
            return await _client.GetUsersByName(firstName, lastName, source.Token);
        }

        /// <summary>
        /// This calls the API and creates an announcement request entry in the database.
        /// </summary>
        /// <param name="announcement">The announcement to be posted.</param>
        /// <returns>The successfully created announcement request.</returns>
        public async Task<AnnouncementRequest> CreateAnnouncement([Body] AnnouncementRequest announcement)
        {
            return await _client.CreateAnnouncement(announcement, source.Token);
        }

        /// <summary>
        /// This calls the API and returns all of the events in the event table.
        /// </summary>
        /// <returns>An array of all of the events.</returns>
        public async Task<Event[]> GetEvents()
        {
            return await _client.GetEvents();
        }

        /// <summary>
        /// This calls the API and creates an event registration entry in the database.
        /// </summary>
        /// <param name="registration">The event registration entry to be posted.</param>
        /// <returns>The successfully created event registration.</returns>
        public async Task<EventRegistration> CreateRegistration([Body] EventRegistration registration)
        {
            return await _client.CreateRegistration(registration, source.Token);
        }

        /// <summary>
        /// This calls the API to update an already-existing event in the database.
        /// </summary>
        /// <param name="eventId">The ID of the event to be updated.</param>
        /// <param name="event">The event model to be updated.</param>
        /// <returns>The successfully updated event.</returns>
        public async Task<Event> UpdateEvent(int eventId, [Body] Event @event)
        {
            return await _client.UpdateEvent(eventId, @event, source.Token);
        }

        /// <summary>
        /// This calls the API and creates a user entry in the database.
        /// </summary>
        /// <param name="user">The user entry to be posted.</param>
        /// <returns>The successfully created user.</returns>
        public async Task<User> CreateUser([Body] User user)
        {
            return await _client.CreateUser(user, source.Token);
        }

        /// <summary>
        /// This calls the API to update an already-existing user in the database.
        /// </summary>
        /// <param name="userId">The ID of the user to be updated.</param>
        /// <param name="user">The user model to be updated.</param>
        /// <returns>The successfully updated event.</returns>
        public async Task<User> UpdateUser(int userId, [Body] User user)
        {
            return await _client.UpdateUser(userId, user, source.Token);
        }

        /// <summary>
        /// This calls the API and creates a prayer request entry in the database.
        /// </summary>
        /// <param name="prayerRequest">The event registration entry to be posted.</param>
        /// <returns>The successfully created prayer request.</returns>
        public async Task<PrayerRequest> CreatePrayerRequest([Body] PrayerRequest prayerRequest)
        {
            return await _client.CreatePrayerRequest(prayerRequest, source.Token);
        }

        /// <summary>
        /// This calls the API and creates visit request entry in the database.
        /// </summary>
        /// <param name="visitRequest">The event registration entry to be posted.</param>
        /// <returns>The successfully created visit request.</returns>
        public async Task<VisitRequest> CreateVisitRequest([Body] VisitRequest visitRequest)
        {
            return await _client.CreateVisitRequest(visitRequest, source.Token);
        }
    }
}
