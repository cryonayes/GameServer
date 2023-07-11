namespace GameServer.Common;

// Sent from master server to client
public enum MasterToClient
{
    Welcome = 1,
    LoginSuccess,
    LoginFailed,
    GoJoinLobby,
}

// Sent from client to master server.
public enum ClientToMaster
{
    Login = 1,
    LobbyRequest
}

// Sent from game server to master server
public enum GameServerToMaster
{
    Welcome = 1,
    LobbyReady
}

public enum MasterToGameServer
{
    LobbyInfo = 1
}