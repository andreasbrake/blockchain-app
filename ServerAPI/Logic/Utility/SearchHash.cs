using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAppAPI.Logic.Utility
{
    public class SearchHash
    {
        public static string HashSearch(string queryString)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] input = Encoding.UTF8.GetBytes(queryString);
                byte[] output = sha1.ComputeHash(input);
                return Convert.ToBase64String(output);
            }
        }
    }
}