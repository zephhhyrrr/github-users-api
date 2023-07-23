using GitHubUsers.Models;
using GitHubUsers.Service;
using Microsoft.AspNetCore.Mvc;

namespace GitHubUsers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GitHubUsersController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public GitHubUsersController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpPost("retrieveUsers")]
        public async Task<ActionResult<List<User?>>> RetrieveUsers(List<string> usernames)
        {
            var distinctUsernames = usernames.Distinct().ToList();
            var users = new List<User?>();

            foreach (var username in distinctUsernames)
            {
                try
                {
                    var user = await _gitHubService.GetUserInfoAsync(username);
                    if (user != null)
                    { 
                        users.Add(user);
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle bad request or other exceptions here
                    return BadRequest(ex.Message);
                }
            }

            // Sort users alphabetically by name
            users = users.OrderBy(u => u?.Name).ToList();

            return Ok(users);
        }
    }
}