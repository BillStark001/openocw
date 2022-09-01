using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Database.Models;
using System;

namespace Oocw.Database;

public class Database
{
    public static readonly Database Instance = new();

    private readonly FilterDefinitionBuilder<BsonDocument> _f;

    private DBWrapper? _wrapper;
    public DBWrapper Wrapper => _wrapper ?? throw new InvalidOperationException("Database is not initialized yet.");

    public IMongoCollection<User> Users => Wrapper.Users;
    public IMongoCollection<Relationship> Relations => Wrapper.Relations;

    protected Database()
    {
        // TODO read config
        _f = Builders<BsonDocument>.Filter;
    }

    public void Initialize(string host = DBWrapper.DEFAULT_HOST)
    {
        _wrapper = new DBWrapper(host);
    }

    // utils
    // TODO add cache (dict & watch)

}

public class DatabaseInternalException: Exception
{

}
