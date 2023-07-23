using GitHubUsers.Models;

namespace GitHubUsers.Service
{
    public interface IGitHubService
    {
        Task<User?> GetUserInfoAsync(string username);
    }
}
