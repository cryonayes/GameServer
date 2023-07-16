namespace GameServer.Common;

// Sent from master server to client

public enum GameServerToClient
{
    Welcome = 20,
    AddPlayers,
    PlayerMove
}
    
public enum ClientToGameServer
{
    JoinLobby = 30,
    PlayerMove,
    OnFinishLine,
}

public enum MasterToGameServer
{
    LobbyInfo = 50
}

public enum GameServerToMaster
{
    Welcome = 40,
    LobbyReady,
    PlayerConnected,
    PlayerDisconnected
}
