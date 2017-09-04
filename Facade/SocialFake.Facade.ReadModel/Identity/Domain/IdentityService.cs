using System;
using System.Net.Http;
using System.Threading.Tasks;
using SocialFake.Identity.Commands;

namespace SocialFake.Identity.Domain
{
    public class IdentityService
    {
        private readonly HttpClient _httpClient;

        public IdentityService(Uri hostAddress)
        {
            if (hostAddress == null)
            {
                throw new ArgumentNullException(nameof(hostAddress));
            }

            _httpClient = new HttpClient { BaseAddress = hostAddress };
        }

        public async Task ExecuteCommand(CreateUserWithPassword command)
        {
            var path = $"commands/{nameof(CreateUserWithPassword)}";
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(path, command);
            response.EnsureSuccessStatusCode();
        }
    }
}
