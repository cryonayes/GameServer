using System.Collections.Concurrent;
using System.Net;
using GameServer.Common;
using GameServer.ServerSide;
using GameServer.Threading;

namespace GameServer.ClientSide;

public class UdpConnection
{
    public IPEndPoint EndPoint;

    private readonly int _id;

    public UdpConnection(int id)
    {
        _id = id;
    }

    public void Connect(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
    }

    public void SendData(Packet packet)
    {
        Server.SendUDPData(EndPoint, packet);
    }

    public void HandleData(Packet packetData)
    {
        var packetLength = packetData.ReadInt();
        var packetBytes = packetData.ReadBytes(packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using var packet = new Packet(packetBytes);
            var packetId = packet.ReadInt();
            Server.PacketHandlers[packetId](_id, packet);
        });
    }
}