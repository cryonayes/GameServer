using System.Net;
using System.Net.Sockets;
using GameServer.ClientSide;
using GameServer.Common;

namespace GameServer.ServerSide
{
    static class Server
    {
        private static int Port { get; set; }
        public static readonly Dictionary<int, Client> Clients = new();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        private static TcpListener _tcpListener;
        private static UdpClient udpListener;

        public static void Start(int _port)
        {
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);
            
            Console.WriteLine($"Server started on port {Port}.");
        }

        private static void TcpConnectCallback(IAsyncResult _result)
        {
            var client = _tcpListener.EndAcceptTcpClient(_result);
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

            var _i = 1;
            while (true)
            {
                if (Clients.ContainsKey(_i))
                {
                    _i++;
                    continue;
                }
                Clients[_i] = new Client(_i);
                Clients[_i].tcp.Connect(client);
                return;
            }
        }

        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                    return;

                using Packet packet = new Packet(data);
                var clientId = packet.ReadInt();

                if (clientId == 0)
                    return;

                if (Clients[clientId].udp.endPoint == null)
                {
                    // If this is a new connection
                    Clients[clientId].udp.Connect(clientEndPoint);
                    return;
                }

                if (Clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                {
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    Clients[clientId].udp.HandleData(packet);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }
        
        private static void InitializeServerData()
        {
            PacketHandlers = new Dictionary<int, PacketHandler>
            {
                { (int)MasterToGameServer.LobbyInfo, ClientHandle.LobbyRequestReceived },
                { (int)ClientToGameServer.JoinLobby, ClientHandle.LoginReceivedToLobby },
                { (int)ClientToGameServer.PlayerMove, ClientHandle.OnPlayerMove },
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
