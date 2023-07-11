namespace MasterServer
{
    internal static class Constants
    {
        public const int TicksPerSec = 10;
        public const float MsPerTick = 1000f / TicksPerSec;
    }

    internal static class Globals
    {
        public static string GameServerIp = "127.0.0.1";
        public static int GameServerMasterPort = 1338;        
        public static int GameServerClientPort = 1339;        
        
        public static string MongoUri = "mongodb+srv://casestudy:case\"100@panteoncasestudy.ofcnjru.mongodb.net/?retryWrites=true&w=majority";
        public static string DatabaseName = "panteon";
        public static string CollectionName = "users";
        
    }
}
