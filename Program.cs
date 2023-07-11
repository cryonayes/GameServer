// See https://aka.ms/new-console-template for more information

using GameServer.ServerSide;
using GameServer.Threading;
using MasterServer;

var mainThread = new Thread(MainThread);
mainThread.Start();

Server.Start(1338);

static void MainThread()
{
    Console.WriteLine($"Main thread started. Running at {Constants.TicksPerSec} ticks per second.");
    var _nextLoop = DateTime.Now;

    while (true)
    {
        while (_nextLoop < DateTime.Now)
        {
            ThreadManager.UpdateMain();
            _nextLoop = _nextLoop.AddMilliseconds(Constants.MsPerTick);

            if (_nextLoop > DateTime.Now)
                Thread.Sleep(_nextLoop - DateTime.Now);
        }
    }
}