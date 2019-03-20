using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Logic.Data
{
    public interface IDataManager
    {
        Task PushToStore(string key, JObject data);
        Task<BaseDataModel> LookupObject(string moduleName, string objectName, string objectId);
        Task RequestData(string moduleName, string objectName, string objectId);
    }
}