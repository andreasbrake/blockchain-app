namespace BlockchainAppAPI.Models.Authentication
{
    public class UserModel 
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }
}