using GameServer.ServerSide;
using GameServer.ServerSide.Lobby;
using MasterServer;

namespace GameServer.ClientSide
{
    class Client
    {
        public const int DataBufferSize = 4096;
        public readonly TcpConnection tcp;
        public readonly UdpConnection udp;
        private int _clientId;
        public Client(int clientId)
        {
            _clientId = clientId;
            tcp = new TcpConnection(clientId);
            udp = new UdpConnection(clientId);
        }
        
        public void Disconnect()
        {
            ClientSend.PlayerDisconnect(Globals.MasterServer);
            var lobby = LobbyManager.GetInstance().GetLobyWithPlayerId(_clientId);
            lobby?.RemovePlayer(_clientId);
            
            Console.WriteLine($"{tcp.Socket.Client.RemoteEndPoint} has disconnected.");
            tcp.Disconnect();
        }
    }
}
