using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class Course : IMergable<Course>
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Metadata.KEY_META)]
    public Metadata Meta { get; set; } = null!;


    public double AccessRank { get; set; }


    public string Code { get; set; } = "";

    public bool IsLink { get; set; }



    public MultiLingualField Name { get; set; } = new();

    public (double, double, double, int) Credit { get; set; }

    public MultiLingualField Unit { get; set; } = new();

    public IEnumerable<int> Classes { get; set; } = new List<int>();


    public UpdateDefinition<P> GetMergeDefinition<P>(Func<P, Course> expr)
    {
        var metaUpdate = Meta.GetMergeDefinition<P>(x => expr(x).Meta);
        var nameUpdate = Name.GetMergeDefinition<P>(x => expr(x).Name);
        var unitUpdate = Unit.GetMergeDefinition<P>(x => expr(x).Unit);
        var selfUpdate = Builders<P>.Update
            .Set(x => expr(x).Credit, Credit)
            .Set(x => expr(x).Classes, Classes);
        return Builders<P>.Update.Combine(metaUpdate, nameUpdate, unitUpdate, selfUpdate);
    }
}

public static class CourseExtensions
{
    public static async Task<Course?> FindCourseAsync(this DBWrapper db, string courseCode, CancellationToken token = default)
    {
        var cursor =
            db is DBSessionWrapper dbSess ?
            await dbSess.Courses.FindAsync(dbSess.Session, x => x.Code == courseCode, cancellationToken: token) :
            await db.Courses.FindAsync(x => x.Code == courseCode, cancellationToken: token);
        return await cursor.FirstOrDefaultAsync(token);
    }

    public static async Task UpdateCourseAsync(this DBWrapper db, Course c, CancellationToken token = default)
    {
        var merge = c.GetMergeDefinition();
        UpdateResult def;
        if (db is DBSessionWrapper dbSess)
            def = await dbSess.Courses.UpdateOneAsync(dbSess.Session, x => x.Code == c.Code, merge, cancellationToken: token);
        else
            def = await db.Courses.UpdateOneAsync(x => x.Code == c.Code, merge, cancellationToken: token);

        if (def.MatchedCount > 0)
            return;
        else
        {
            // update matching failed, insert
            if (db is DBSessionWrapper dbSess2)
                await dbSess2.Courses.InsertOneAsync(dbSess2.Session, c, cancellationToken: token);
            else
                await db.Courses.InsertOneAsync(c, cancellationToken: token);
        }
    }
}