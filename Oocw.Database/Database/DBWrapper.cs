using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;
using Oocw.Database.Models;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Serializer.ValueTuple;

namespace Oocw.Database;

public class DBWrapper
{
    public delegate TResult TransactionCallback<TResult>(DBSessionWrapper wrapper, CancellationToken token);

    protected IMongoClient _client;
    protected IMongoDatabase _database;
    protected IMongoCollection<User> _users;
    protected IMongoCollection<Relationship> _relations;
    protected IMongoCollection<Counter> _counters;

    public IMongoClient Client => _client;
    public IMongoCollection<User> Users => _users;
    public IMongoCollection<Relationship> Relations => _relations;

    protected IMongoCollection<BsonDocument> _classes;
    protected IMongoCollection<Course> _courses;
    protected IMongoCollection<Faculty> _faculties;

    public IMongoCollection<BsonDocument> Classes => _classes;
    public IMongoCollection<Course> Courses => _courses;
    public IMongoCollection<Faculty> Faculties => _faculties;

    private static readonly IEnumerable<string> DBNames;
    private static readonly FilterDefinitionBuilder<BsonDocument> F;
    public const string DEFAULT_HOST = "mongodb://localhost:27017/";


    static DBWrapper()
    {
        DBNames = new List<string>
        {
            Definitions.COL_USER_NAME, 
        }.AsReadOnly();
        F = Builders<BsonDocument>.Filter;
    }

    private static bool Reg = false;

    protected DBWrapper(IMongoClient client)
    {
        if (!Reg)
        {
            ValueTupleSerializerRegistry.Register();
            Reg = true;
        }

        _client = client;

        _database = _client.GetDatabase(Definitions.DB_SET_NAME);
        _counters = _database.GetCollection<Counter>(Definitions.COL_CNT_NAME);

        _users = _database.GetCollection<User>(Definitions.COL_USER_NAME);
        _relations = _database.GetCollection<Relationship>(Definitions.COL_REL_NAME);

        _classes = _database.GetCollection<BsonDocument>(Definitions.COL_CLASS_NAME);
        _courses = _database.GetCollection<Course>(Definitions.COL_COURSE_NAME);
        _faculties = _database.GetCollection<Faculty>(Definitions.COL_FACULTY_NAME);

    }

    protected static MongoClient GetClientByString(string connStr = DEFAULT_HOST)
    {
        var settings = MongoClientSettings.FromConnectionString(connStr);
        // settings.ServerApi = new(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        return client;
    }

    public DBWrapper(string connStr = DEFAULT_HOST) : this(GetClientByString(connStr)) {

        // check if counter is properly initialized
        foreach (var dbName in DBNames)
        {
            if (_counters.Find(x => x.DBName == dbName).CountDocuments() == 0)
                _counters.InsertOne(new(dbName));
        }
    }

    public void ResetCounter()
    {
        _database.DropCollection(Definitions.COL_CNT_NAME);
        _counters = _database.GetCollection<Counter>(Definitions.COL_CNT_NAME);
        foreach (var dbName in DBNames)
        {
            if (_counters.Find(x => x.DBName == dbName).CountDocuments() == 0)
                _counters.InsertOne(new(dbName));
        }
    }


    // incremental ids

    public virtual int GetIncrementalId(string target)
    {
        var orig = _counters.FindOneAndUpdate(x => x.DBName == target, Builders<Counter>.Update.Inc(x => x.Sequel, 1));
        return orig.Sequel;
    }

    public int GetUserIncrementalId() => GetIncrementalId(Definitions.COL_USER_NAME);


    public virtual async Task<int> GetIncrementalIdAsync(string target)
    {
        var orig = await _counters.FindOneAndUpdateAsync(x => x.DBName == target, Builders<Counter>.Update.Inc(x => x.Sequel, 1));
        return orig.Sequel;
    }

    public async Task<int> GetUserIncrementalIdAsync() => await GetIncrementalIdAsync(Definitions.COL_USER_NAME);


    public TResult? UseTransaction<TResult>(
        TransactionCallback<TResult> callback,
        CancellationToken? cToken = null)
    {
        using (var sess = _client.StartSession())
        {
            var options = new TransactionOptions(
                readPreference: ReadPreference.Primary,
                readConcern: ReadConcern.Local,
                writeConcern: WriteConcern.WMajority
                );

            var result = sess.WithTransaction(
                (s, c) => callback(new DBSessionWrapper(s), c), 
                options,
                cToken ?? CancellationToken.None
            );
            return result;
        }
    }

    public async Task<TResult?> UseTransactionAsync<TResult>(
        TransactionCallback<Task<TResult>> callback,
        CancellationToken? cToken = null)
    {
        using (var sess = await _client.StartSessionAsync())
        {
            var options = new TransactionOptions(
                readPreference: ReadPreference.Primary,
                readConcern: ReadConcern.Local,
                writeConcern: WriteConcern.WMajority
                );

            var result = await sess.WithTransactionAsync(
                (s, c) => callback(new DBSessionWrapper(s), c),
                options,
                cToken ?? CancellationToken.None
            );
            return result;
        }
    }
}