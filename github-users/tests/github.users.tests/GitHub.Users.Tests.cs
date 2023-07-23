using GitHubUsers.Controllers;
using GitHubUsers.Models;
using GitHubUsers.Service;
using System.Net;
using System.Text.Json;

namespace github.users.tests
{
    public class GitHubUsersControllerTests
    {

        [Fact]
        public async Task RetrieveUsers_ShouldReturnSortedUsers()
        {
            // Arrange
            var mockGitHubService = new Mock<IGitHubService>();
            var gitHubUsersController = new GitHubUsersController(mockGitHubService.Object);

            var usernames = new List<string>
            {
                "mojombo",
                "defunkt",
                "defunkt", // Duplicate username to test distinct filtering
                "pjhyett"
            };

            var user1 = new User { Login = "mojombo" , Name = "Tom Preston-Werner" };
            var user2 = new User { Login = "defunkt" , Name = "Chris Wanstrath" };
            var user3 = new User { Login = "pjhyett" , Name = "PJ Hyett" };

            // Set up the mock for GetUserInfoAsync method
            mockGitHubService
                .Setup(service => service.GetUserInfoAsync("mojombo"))
                .ReturnsAsync(user1);
            mockGitHubService
                .Setup(service => service.GetUserInfoAsync("defunkt"))
                .ReturnsAsync(user2);
            mockGitHubService
                .Setup(service => service.GetUserInfoAsync("pjhyett"))
                .ReturnsAsync(user3);

            // Act
            var result = await gitHubUsersController.RetrieveUsers(usernames);

            // Assert
            result?.Should().NotBeNull();
            result?.Value?.Count.Should().Be(3); // Distinct usernames: user1, user2, user3
            result?.Value?[0]?.Login.Should().Be("defunkt"); // Sorted alphabetically
            result?.Value?[1]?.Login.Should().Be("pjhyett");
            result?.Value?[2]?.Login.Should().Be("mojombo");
        }
        
        
        [Fact]
        public async Task RetrieveUsers_ShouldReturnEmptyList()
        {
            // Arrange
            var mockGitHubService = new Mock<IGitHubService>();
            var gitHubUsersController = new GitHubUsersController(mockGitHubService.Object);

            var usernames = new List<string>
            {
                "wycats"
            };

            var user1 = new User { Login = "mojombo" };
            var user2 = new User { Login = "defunkt" };
            var user3 = new User { Login = "pjhyett" };

            // Set up the mock for GetUserInfoAsync method
            mockGitHubService
                .Setup(service => service.GetUserInfoAsync("mojombo"))
                .ReturnsAsync(user1);
            mockGitHubService
                .Setup(service => service.GetUserInfoAsync("defunkt"))
                .ReturnsAsync(user2);
            mockGitHubService
                .Setup(service => service.GetUserInfoAsync("pjhyett"))
                .ReturnsAsync(user3);

            // Act
            var result = await gitHubUsersController.RetrieveUsers(usernames);

            // Assert
            result?.Value.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetUserInfoAsync_ShouldReturnValidUser()
        {
            // Arrange
            var username = "mojombo";
            var expectedUser = new User
            {
                Login = "mojombo",
                Name = "Tom Preston-Werner",
                // Set other properties as needed
            };

            var httpClientMock = new Mock<IHttpClientWrapper>();
            var gitHubService = new GitHubService(httpClientMock.Object);

            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var responseContent = JsonSerializer.Serialize(expectedUser);
            apiResponse.Content = new StringContent(responseContent);

            httpClientMock
                .Setup(client => client.GetAsync($"https://api.github.com/users/{username}"))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await gitHubService.GetUserInfoAsync(username);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUserInfoAsync_ShouldReturnNullOnApiFailure()
        {
            // Arrange
            var username = "asdasdasd";
            var httpClientMock = new Mock<IHttpClientWrapper>();
            var gitHubService = new GitHubService(httpClientMock.Object);

            var apiResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            httpClientMock
                .Setup(client => client.GetAsync($"https://api.github.com/users/{username}"))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await gitHubService.GetUserInfoAsync(username);

            // Assert
            result?.Name.Should().BeNullOrEmpty();
            result?.Login.Should().BeNullOrEmpty();
            result?.Company.Should().BeNullOrEmpty();
            result?.Followers.Should().Be(0);
            result?.Public_Repos.Should().Be(0);
            result?.AvgFollowersPerRepo.Should().Be(0);
        }
    }
}