using MongoDB.Bson;
using MongoDB.Driver;

namespace GameServer.Database;

public static class MongoCrud
{
    private static IMongoCollection<BsonDocument> _collection;

    public static void Connect(string _connectionUri, string _databaseName, string _collectionName)
    {
        var _database = new MongoClient(_connectionUri).GetDatabase(_databaseName);
        _collection = _database.GetCollection<BsonDocument>(_collectionName);
    }

    public static void Create(BsonDocument _document)
    {
        _collection.InsertOne(_document);
    }

    public static List<BsonDocument> Read(FilterDefinition<BsonDocument> _filter)
    {
        return _collection.Find(_filter).ToList();
    }

    public static void Update(FilterDefinition<BsonDocument> _filter, UpdateDefinition<BsonDocument> update)
    {
        _collection.UpdateOne(_filter, update);
    }

    public static void Delete(FilterDefinition<BsonDocument> _filter)
    {
        _collection.DeleteOne(_filter);
    }
}