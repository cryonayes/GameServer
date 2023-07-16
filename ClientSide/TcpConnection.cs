using System.Net.Sockets;
using GameServer.Common;
using GameServer.ServerSide;
using GameServer.ServerSide.Lobby;
using GameServer.Threading;

namespace GameServer.ClientSide;

public class TcpConnection
{
    public TcpClient? Socket;
    

    private readonly int _id;
    private NetworkStream? _stream;
    private Packet? _receivedData;
    private byte[]? _receiveBuffer;

    public TcpConnection(int id)
    {
        _id = id;
    }

    public void Connect(TcpClient socket)
    {
        Socket = socket;
        Socket.ReceiveBufferSize = Client.DataBufferSize;
        Socket.SendBufferSize = Client.DataBufferSize;

        _stream = Socket.GetStream();
        _receivedData = new Packet();
        _receiveBuffer = new byte[Client.DataBufferSize];

        _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        ClientSend.Welcome(_id);
    }

    public void SendData(Packet packet)
    {
        try
        {
            if (Socket != null)
                _stream?.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Send data to appropriate client
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data to player {_id} via TCP: {ex}");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            if (_stream == null) return;
            var byteLength = _stream.EndRead(result);
            if (byteLength <= 0)
            {
                Server.Clients[_id].Disconnect();
                return;
            }

            var data = new byte[byteLength];
            if (_receiveBuffer == null) return;
            Array.Copy(_receiveBuffer, data, byteLength);

            _receivedData?.Reset(HandleData(data));
            _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving TCP data: {ex}");
            Server.Clients[_id].Disconnect();

        }
    }

    private bool HandleData(byte[] data)
    {
        var packetLength = 0;
        
        _receivedData.SetBytes(data);
        if (_receivedData.UnreadLength() >= 4)
        {
            packetLength = _receivedData.ReadInt();
            if (packetLength <= 0)
                return true; 
        }

        while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
        {
            var packetBytes = _receivedData.ReadBytes(packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using var packet = new Packet(packetBytes);
                var packetId = packet.ReadInt();
                Server.PacketHandlers[packetId](_id, packet); // Call appropriate method to handle the packet
            });

            packetLength = 0; // Reset packet length
            if (_receivedData.UnreadLength() < 4) continue;
            packetLength = _receivedData.ReadInt();
            if (packetLength <= 0)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }
        }

        return packetLength <= 1;
        // Reset receivedData instance to allow it to be reused
    }

    public void Disconnect()
    {
        Socket?.Close();
        _stream = null;
        _receivedData = null;
        _receiveBuffer = null;
        Socket = null;
    }
}