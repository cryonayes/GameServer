using System.Numerics;
using GameServer.Common;
using GameServer.ServerSide.Lobby;

namespace GameServer.ServerSide
{
    internal abstract class ClientSend
    {
        private static void SendTcpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClient].Tcp.SendData(packet);
        }
        
        private static void SendUdpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClient].Udp.SendData(packet);
        }

        #region Packets
        
        public static void Welcome(int toClient)
        {
            using var packet = new Packet((int)GameServerToClient.Welcome);
            packet.Write(toClient);
            SendTcpData(toClient, packet);
        }
        
        public static void LobbyReady(int toClient, string lobbyId)
        {
            var packet = new Packet((int)GameServerToMaster.LobbyReady);
            packet.Write(lobbyId);
            
            SendTcpData(toClient, packet);
        }
        
        public static void SpawnPlayers(LobbyManager.Lobby lobby)
        {
            Console.WriteLine("Spawn players.");
            var players = lobby.GetPlayers();

            using var packet = new Packet();
            foreach (var to in players)
            {
                foreach (var who in players)
                {
                    packet.Write((int)GameServerToClient.AddPlayers);
                    packet.Write(who);
                    packet.Write(new Vector3(-10f, -5f, 1f));
                    SendTcpData(to, packet);
                    packet.Reset();
                }
            }
        }

        public static void PlayerMove(int toClient, int movingId, Vector3 move)
        {
            using var packet = new Packet((int) GameServerToClient.PlayerMove);
            packet.Write(movingId);
            packet.Write(move);

            SendUdpData(toClient, packet);
        }
        
        public static void PlayerConnected(int masterServer, int playerId)
        {
            using var packet = new Packet((int)GameServerToMaster.PlayerConnected);
            packet.Write(playerId);
            SendTcpData(masterServer, packet);
        }
        
        public static void PlayerDisconnect(int masterServer, int playerId)
        {
            using var packet = new Packet((int)GameServerToMaster.PlayerDisconnected);
            packet.Write(playerId);
            SendTcpData(masterServer, packet);
        }
        
        #endregion
    }
}
