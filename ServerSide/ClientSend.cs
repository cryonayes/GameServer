using GameServer.Common;
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

        #region Packets
        public static void Welcome(int _toClient, string _message)
        {
            using var _packet = new Packet((int)MasterToClient.Welcome);
            _packet.Write(_message);
            SendTcpData(_toClient, _packet);
        }
        
        public static void LoginFailed(int _toClient)
        {
            using var _packet = new Packet((int)MasterToClient.LoginFailed);
            SendTcpData(_toClient, _packet);
        }
        public static void LoginSuccess(int _toClient, string _username, string _token)
        {
            using var _packet = new Packet((int)MasterToClient.LoginSuccess);
            _packet.Write(_username);
            _packet.Write(_token);
            _packet.Write(Globals.GameServerIp);
            _packet.Write(Globals.GameServerClientPort);
            
            SendTcpData(_toClient, _packet);
        }

        public static void JoinToLobby(int _toClient, string _lobbyId)
        {
            using var _packet = new Packet((int)MasterToClient.GoJoinLobby);
            _packet.Write(_lobbyId);
            
            SendTcpData(_toClient, _packet);
        }

        #endregion
    }
}
