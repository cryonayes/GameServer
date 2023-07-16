using GameServer.ServerSide.Lobby;

namespace GameServer.ClientSide
{
    class Client
    {
        public const int DataBufferSize = 4096;
        public readonly TcpConnection Tcp;
        public readonly UdpConnection Udp;
        private readonly int _clientId;
        
        public Client(int clientId)
        {
            _clientId = clientId;
            Tcp = new TcpConnection(clientId);
            Udp = new UdpConnection(clientId);
        }
        
        public void Disconnect()
        {
            var lobby = LobbyManager.GetInstance().GetLobbyWithPlayerId(_clientId);
            lobby?.RemovePlayer(_clientId);
            
            Console.WriteLine($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");
            Tcp.Disconnect();
        }
    }
}
