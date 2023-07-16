namespace GameServer.ServerSide.Lobby;

public class LobbyManager
{
    private static LobbyManager? _instance;
    private readonly List<Lobby> _lobbies;
    
    private LobbyManager()
    {
        _lobbies = new List<Lobby>();
    }
    
    public static LobbyManager GetInstance()
    {
        return _instance ??= new LobbyManager();
    }
    
    public Lobby GetOrCreateLobby(string id, int capacity)
    {
        var lobby = _lobbies.Find(lobby => lobby.GetId().Equals(id) && lobby.Capacity == capacity);
        if (lobby != null)
            return lobby;
        _lobbies.Add(new Lobby(id, capacity));
        return _lobbies[^1];
    }

    private void OnLobbyFull(Lobby lobby)
    {
        ClientSend.SpawnPlayers(lobby);
    }
    
    public Lobby? GetLobyWithPlayerId(int id)
    {
        foreach (var lobby in _lobbies)
            foreach (var playerId in lobby.GetPlayers())
                if (playerId == id)
                    return lobby;
        return null;
    }
    
    private void OnLobbyEmpty(Lobby lobby)
    {
        _lobbies.Remove(lobby);
    }
    
    public class Lobby
    {
        private readonly string _lobbyId;
        private readonly List<int> _players;
        public readonly int Capacity;
        
        public Lobby(string id, int maxPlayers)
        {
            _players = new ();
            _lobbyId = id;
            Capacity = maxPlayers;
        }
        
        public void AddPlayer(int _playerId)
        {
            _players.Add(_playerId);
            if (Capacity == _players.Count)
                _instance.OnLobbyFull(this);
        }

        public string GetId()
        {
            return _lobbyId;
        }
        
        public List<int> GetPlayers()
        {
            return _players;
        }

        public void RemovePlayer(int id)
        {
            if (!_players.Remove(id)) return;
            if (_players.Count == 0)
                _instance?.OnLobbyEmpty(this);
        }
    }
}