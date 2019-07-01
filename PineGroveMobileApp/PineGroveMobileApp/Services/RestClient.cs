﻿using Refit;
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
    }
}
