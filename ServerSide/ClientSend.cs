using System.Numerics;
using GameServer.Common;
using GameServer.ServerSide.Lobby;
using MasterServer;

namespace GameServer.ServerSide
{
    internal abstract class ClientSend
    {
        private static void SendTcpData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.Clients[_toClient].tcp.SendData(_packet);
        }
        
        private static void SendUdpData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.Clients[_toClient].udp.SendData(_packet);
        }

        #region Packets
        
        public static void Welcome(int _toClient)
        {
            using var _packet = new Packet((int)GameServerToClient.Welcome);
            _packet.Write(_toClient);
            SendTcpData(_toClient, _packet);
        }
        
        public static void LobbyReady(int _toClient, string _lobbyId)
        {
            var packet = new Packet((int)GameServerToMaster.LobbyReady);
            packet.Write(_lobbyId);
            
            SendTcpData(_toClient, packet);
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

        public static void PlayerMove(int _toClient, int movingID, Vector3 move)
        {
            using var packet = new Packet((int) GameServerToClient.PlayerMove);
            packet.Write(movingID);
            packet.Write(move);

            SendUdpData(_toClient, packet);
        }
        
        public static void PlayerConnected(int masterServer)
        {
            using var packet = new Packet((int)GameServerToMaster.PlayerConnected);
            packet.Write("Connected");
            SendTcpData(masterServer, packet);
        }
        
        public static void PlayerDisconnect(int masterServer)
        {
            using var packet = new Packet((int)GameServerToMaster.PlayerDisconnected);
            packet.Write("Disconnected");
            SendTcpData(masterServer, packet);
        }
        
        #endregion
    }
}
