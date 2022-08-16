using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;

namespace Oocw.Backend.Database
{
    public class DBWrapper
    {
        public delegate TResult TransactionCallback<TResult>(DBSessionWrapper wrapper, CancellationToken token);

        protected IMongoClient _client;
        protected IMongoDatabase _database;
        protected IMongoCollection<BsonDocument> _classes;
        protected IMongoCollection<BsonDocument> _courses;
        protected IMongoCollection<BsonDocument> _faculties;

        public IMongoClient Client => _client;
        public IMongoCollection<BsonDocument> Classes => _classes;
        public IMongoCollection<BsonDocument> Courses => _courses;
        public IMongoCollection<BsonDocument> Faculties => _faculties;

        protected DBWrapper(IMongoClient client)
        {
            _client = client;

            _database = _client.GetDatabase(Definitions.DB_SET_NAME);
            _classes = _database.GetCollection<BsonDocument>(Definitions.COL_CLASS_NAME);
            _courses = _database.GetCollection<BsonDocument>(Definitions.COL_COURSE_NAME);
            _faculties = _database.GetCollection<BsonDocument>(Definitions.COL_FACULTY_NAME);
        }

        protected static MongoClient GetClientByString(string connStr = "mongodb://localhost:27017/")
        {
            var settings = MongoClientSettings.FromConnectionString(connStr);
            settings.ServerApi = new(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            return client;
        }

        public DBWrapper(string connStr = "mongodb://localhost:27017/"): this(GetClientByString(connStr)) { }

        public void UseTransaction<TResult>(
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

                var result = sess.WithTransaction((s, c) =>
                {
                    using (var dbSess = new DBSessionWrapper(s))
                    {
                        return callback(dbSess, c);
                    }
                }, 
                options, 
                cToken ?? CancellationToken.None
                );
            }
        }
    }
}
