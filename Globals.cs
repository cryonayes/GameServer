namespace MasterServer
{
    internal static class Constants
    {
        public const int TicksPerSec = 60;
        public const float MsPerTick = 1000f / TicksPerSec;
    }

    internal static class Globals
    {
        public static int MasterServer = 0;
        
        public static string MongoUri = "mongodb+srv://casestudy:case\"100@panteoncasestudy.ofcnjru.mongodb.net/?retryWrites=true&w=majority";
        public static string DatabaseName = "panteon";
        public static string CollectionName = "users";
    }
}
