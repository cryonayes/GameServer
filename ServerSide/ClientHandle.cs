using GameServer.Common;
using GameServer.ServerSide.Lobby;
using MasterServer;

namespace GameServer.ServerSide
{
    internal abstract class ClientHandle
    {
        public static void LoginReceivedToLobby(int fromClient, Packet packet)
        {
            var lobbyId = packet.ReadString();

            var lobby = LobbyManager.GetInstance().GetLobbyById(lobbyId);
            lobby?.AddPlayer(fromClient);
        }

        public static void LobbyRequestReceived(int fromClient, Packet packet)
        {
            var lobbyId = packet.ReadString();
            var capacity = packet.ReadInt();

            Globals.MasterServer = fromClient;
            LobbyManager.GetInstance().GetOrCreateLobby(lobbyId, capacity);
            ClientSend.LobbyReady(fromClient, lobbyId);
        }

        public static void OnPlayerMove(int fromClient, Packet packet)
        {
            var move = packet.ReadVector3();
            var lobby = LobbyManager.GetInstance().GetLobbyWithPlayerId(fromClient);

            if (lobby == null) return;
            foreach (var playerId in lobby.GetPlayers().Where(playerId => playerId != fromClient))
                ClientSend.PlayerMove(playerId, fromClient, move);
        }
    }
}
