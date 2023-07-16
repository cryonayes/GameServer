using GameServer.ServerSide;
using GameServer.Threading;
using MasterServer;

var mainThread = new Thread(MainThread);
mainThread.Start();

Server.Start(1338);

static void MainThread()
{
    Console.WriteLine($"Main thread started. Running at {Constants.TicksPerSec} ticks per second.");
    var nextLoop = DateTime.Now;

    while (true)
    {
        while (nextLoop < DateTime.Now)
        {
            ThreadManager.UpdateMain();
            nextLoop = nextLoop.AddMilliseconds(Constants.MsPerTick);

            if (nextLoop > DateTime.Now)
                Thread.Sleep(nextLoop - DateTime.Now);
        }
    }
}