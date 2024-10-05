using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Serializer.ValueTuple;
using MongoDB.Bson.Serialization.Conventions;
using Oocw.Database.Models;
using Oocw.Database.Models.Technical;
using MongoDB.Bson.Serialization;

namespace Oocw.Database;

public class OocwDatabase
{
    
    public const string DB_SET_NAME = "openocw";
    
    public IMongoClient Client { get; protected set; }
    public IMongoDatabase Database { get; protected set; }


    public IMongoCollection<User> Users { get; protected set; }
    public IMongoCollection<Notification> Notifications { get; protected set; }

    public IMongoCollection<Course> Courses { get; protected set; }
    public IMongoCollection<Class> Classes { get; protected set; }
    public IMongoCollection<ClassInstance> ClassInstances { get; protected set; }

    public IMongoCollection<CourseDiscussion> CourseDiscussions { get; protected set; }
    public IMongoCollection<CourseSelection> CourseSelections { get; protected set; }
    public IMongoCollection<AssignmentSubmission> AssignmentSubmissions { get; protected set; }

    public const string DEFAULT_HOST = "mongodb://localhost:27017/";


    static OocwDatabase()
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };
        ConventionRegistry.Register("CamelCase", pack, t => true);
    }

    private static bool Reg = false;

    protected OocwDatabase(IMongoClient client)
    {
        if (!Reg)
        {
            ValueTupleSerializerRegistry.Register();
            Reg = true;
        }

        Client = client;

        Database = Client.GetDatabase(DB_SET_NAME);

        Users = Database.GetCollection<User>(nameof(User));
        Notifications = Database.GetCollection<Notification>(nameof(Notification));

        // database definition

        Courses = Database.GetCollection<Course>(nameof(Course));
        Classes = Database.GetCollection<Class>(nameof(Class));
        ClassInstances = Database.GetCollection<ClassInstance>(nameof(ClassInstance));

        CourseDiscussions = Database.GetCollection<CourseDiscussion>(nameof(CourseDiscussion));
        CourseSelections = Database.GetCollection<CourseSelection>(nameof(CourseSelection));
        AssignmentSubmissions = Database.GetCollection<AssignmentSubmission>(nameof(AssignmentSubmission));

        // indices

        CreateDataModelUniqueIdIndex(Courses);
        CreateDataModelUniqueIdIndex(Classes);
        CreateDataModelUniqueIdIndex(ClassInstances);

        CreateDataModelUniqueIdIndex(CourseDiscussions);
        CreateDataModelUniqueIdIndex(CourseSelections);
        CreateDataModelUniqueIdIndex(AssignmentSubmissions);

        // text indices

        Courses.Indexes.CreateOne(new CreateIndexModel<Course>(Builders<Course>.IndexKeys.Text(x => x.Meta.SearchRecord)));

    }

    protected static void CreateDataModelUniqueIdIndex<T>(IMongoCollection<T> collection) where T: DataModel{
        var uniqueOptions = new CreateIndexOptions { Unique = true };
        collection.Indexes.CreateOne(new CreateIndexModel<T>(Builders<T>.IndexKeys.Ascending(x => x.Id), uniqueOptions));
    }

    protected static MongoClient GetClientByString(string connStr = DEFAULT_HOST)
    {
        var settings = MongoClientSettings.FromConnectionString(connStr);
        var client = new MongoClient(settings);
        return client;
    }

    public OocwDatabase(string connStr = DEFAULT_HOST) : this(GetClientByString(connStr))
    {

    }

}
