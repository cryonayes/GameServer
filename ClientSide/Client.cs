namespace GameServer.ClientSide
{
    class Client
    {
        public const int DataBufferSize = 4096;
        public readonly TcpConnection tcp;
        public readonly UdpConnection udp;

        public Client(int _clientId)
        {
            tcp = new TcpConnection(_clientId);
            udp = new UdpConnection(_clientId);
        }
        
        public void Disconnect()
        {
            Console.WriteLine($"{tcp.Socket.Client.RemoteEndPoint} has disconnected.");
            tcp.Disconnect();
        }
    }
}
