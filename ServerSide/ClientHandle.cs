using GameServer.Common;
using GameServer.Database;
using GameServer.ServerSide.Lobby;
using MasterServer;

namespace GameServer.ServerSide
{
    internal abstract class ClientHandle
    {
        public static void LoginReceivedToLobby(int _fromClient, Packet _packet)
        {
            var lobbyId = _packet.ReadString();
            
            var lobby = LobbyManager.GetInstance().GetOrCreateLobby(lobbyId, 2);
            lobby.AddPlayer(_fromClient);
            ClientSend.PlayerConnected(Globals.MasterServer);
        }

        public static void LobbyRequestReceived(int _fromClient, Packet _packet)
        {
            var lobbyId = _packet.ReadString();
            var capacity = _packet.ReadInt();

            Globals.MasterServer = _fromClient;
            LobbyManager.GetInstance().GetOrCreateLobby(lobbyId, capacity);
            ClientSend.LobbyReady(_fromClient, lobbyId);
        }

        public static void OnPlayerMove(int _fromClient, Packet _packet)
        {
            var move = _packet.ReadVector3();
            var lobby = LobbyManager.GetInstance().GetLobyWithPlayerId(_fromClient);

            foreach (var playerId in lobby.GetPlayers())
            {
                if (playerId == _fromClient) continue;
                ClientSend.PlayerMove(playerId, _fromClient, move);
            }
         }
    }
}
