using System.Net;
using GameServer.Common;
using GameServer.ServerSide;
using GameServer.Threading;

namespace GameServer.ClientSide;

public class UdpConnection
{
    public IPEndPoint endPoint;

    private int id;

    public UdpConnection(int _id)
    {
        id = _id;
    }

    /// <summary>Initializes the newly connected client's UDP-related info.</summary>
    /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
    public void Connect(IPEndPoint _endPoint)
    {
        endPoint = _endPoint;
    }

    /// <summary>Sends data to the client via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public void SendData(Packet _packet)
    {
        Server.SendUDPData(endPoint, _packet);
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_packetData">The packet containing the recieved data.</param>
    public void HandleData(Packet _packetData)
    {
        int _packetLength = _packetData.ReadInt();
        byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet _packet = new Packet(_packetBytes))
            {
                int _packetId = _packet.ReadInt();
                Server.PacketHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
            }
        });
    }

    /// <summary>Cleans up the UDP connection.</summary>
    public void Disconnect()
    {
        endPoint = null;
    }
}