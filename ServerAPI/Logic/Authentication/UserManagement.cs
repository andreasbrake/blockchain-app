using System.Threading.Tasks;

using BlockchainAppAPI.Logic.Utility;
using BlockchainAppAPI.Models.Authentication;

namespace BlockchainAppAPI.Logic.Authentication
{
    public class UserManager
    {
        public async Task<UserModel> FindByUserNameAsync(string userName)
        {
            return await Task.FromResult<UserModel>(new UserModel() 
            {
                UserName = userName,
                PasswordHash = SaltHash.HashPassword("abc123")
            });
        }
    }
}