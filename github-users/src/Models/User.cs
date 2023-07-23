namespace GitHubUsers.Models
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public int Followers { get; set; }
        public int Public_Repos { get; set; }
        public int AvgFollowersPerRepo => Public_Repos > 0 ? Followers / Public_Repos : 0;
    }
}
