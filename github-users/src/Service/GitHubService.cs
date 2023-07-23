using GitHubUsers.Models;
using System.Text.Json;

namespace GitHubUsers.Service
{
    public class GitHubService: IGitHubService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public GitHubService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<User?> GetUserInfoAsync(string username)
        {

            // use github public api to get user information
            // make a get request to https://api.github.com/users/{username}
            var apiResponse = await _httpClientWrapper.GetAsync($"https://api.github.com/users/{username}");
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseStream = await apiResponse.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var gitHubUser = JsonSerializer.Deserialize<User?>(responseStream, options);
                return gitHubUser ?? new User();
            }
            return null;
        }
    }
}
