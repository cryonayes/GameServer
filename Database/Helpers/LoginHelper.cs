using MongoDB.Bson;
using MongoDB.Driver;

namespace GameServer.Database.Helpers;

public static class LoginHelper
{
    public static string HandleLogin(string _username, string _password)
    {
        var _filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("username", _username),
            Builders<BsonDocument>.Filter.Eq("password", _password)
        );
        var _read = MongoCrud.Read(_filter);
        return _read.Count == 0 ? "" : UpdateAuthToken(_filter);
    }
    
    private static string UpdateAuthToken(FilterDefinition<BsonDocument> _filter)
    {
        var _authToken = Guid.NewGuid().ToString();
        var _update = Builders<BsonDocument>.Update.Set("authToken", _authToken);
        MongoCrud.Update(_filter, _update);
        return _authToken;
    }
}