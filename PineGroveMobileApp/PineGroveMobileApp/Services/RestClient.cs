using System;
using System.Collections.Generic;
using System.Text;
using Refit;
using System.Threading.Tasks;
using System.Net.Http;
using PineGroveMobileApp.Models;

namespace PineGroveMobileApp.Services
{
    public class RestClient
    {
        private readonly IRestable _client;

        public RestClient()
        {
            _client = RestService.For<IRestable>("https://36vhnw37q6.execute-api.us-east-2.amazonaws.com/Prod");
        }

        public async Task<User[]> GetUsers()
        {
            return await _client.GetUsers();
        }

        public async Task<User> GetUser(string username)
        {
            return await _client.GetUser(username);
        }

        public async Task<User[]> GetUsersByName(string firstName, string lastName)
        {
            return await _client.GetUsersByName(firstName, lastName);
        }
    }
}
