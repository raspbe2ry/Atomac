using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomac.RedisDataLayer
{
    public class Config
    {

        public const bool IgnoreLongTests = true;
        public static string host = "pub-redis-11983.eu-central-1-1.1.ec2.redislabs.com";

        public static string SingleHost
        {
            get { return host; }
        }

        public static readonly string[] MasterHosts = new[] { "localhost" };
        public static readonly string[] SlaveHosts = new[] { "localhost" };

        public const int RedisPort = 6379;
        public static int port = 11983;

        public static string SingleHostConnectionString
        {
            get { return SingleHost + ":" + port; }
        }

        public static BasicRedisClientManager BasicClientManger
        {
            get
            {
                return new BasicRedisClientManager(new[] {
                    SingleHostConnectionString
                });
            }
        }

    }
}
