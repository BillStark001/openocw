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

        Database = Client.GetDatabase(Definitions.DB_SET_NAME);

        Users = Database.GetCollection<User>(typeof(User).Name);
        Notifications = Database.GetCollection<Notification>(typeof(Notification).Name);

        Courses = Database.GetCollection<Course>(typeof(Course).Name);
        Classes = Database.GetCollection<Class>(typeof(Class).Name);
        ClassInstances = Database.GetCollection<ClassInstance>(typeof(ClassInstance).Name);


        CourseDiscussions = Database.GetCollection<CourseDiscussion>(typeof(CourseDiscussion).Name);
        CourseSelections = Database.GetCollection<CourseSelection>(typeof(CourseSelection).Name);
        AssignmentSubmissions = Database.GetCollection<AssignmentSubmission>(typeof(AssignmentSubmission).Name);
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
