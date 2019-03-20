using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace BlockchainAppAPI
{
    public class InertialStub
    {
        public static void UpdateObjectValue()
        {
            Task.Run(() => {
                using(ConnectionMultiplexer multiplex = ConnectionMultiplexer.Connect("10.0.75.2:6379"))
                {
                    ISubscriber subscription = multiplex.GetSubscriber();
                    while(true)
                    {
                        JObject newValue = new JObject();
                        newValue["DateModified"] = DateTime.UtcNow;

                        subscription.Publish("patent-application-F19F133B-FCA1-4348-84DF-471DC74E1981", JsonConvert.SerializeObject(newValue));
                        Thread.Sleep(791);
                    }
                }
            });
        }
    }
}