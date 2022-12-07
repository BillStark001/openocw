using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Base;
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


    public UpdateDefinition<P> GetMergeDefinition<P>(Expression<Func<P, Course>> expr)
    {
        var metaUpdate = Meta.GetMergeDefinition<P>(ExpressionUtils.Combine(expr, x => x.Meta));
        var nameUpdate = Name.GetMergeDefinition<P>(ExpressionUtils.Combine(expr, x => x.Name));
        var unitUpdate = Unit.GetMergeDefinition<P>(ExpressionUtils.Combine(expr, x => x.Unit));
        var selfUpdate = Builders<P>.Update
            .Set(ExpressionUtils.Combine(expr, x => x.Credit), Credit)
            .Set(ExpressionUtils.Combine(expr, x => x.Classes), Classes);
        return Builders<P>.Update.Combine(metaUpdate, nameUpdate, unitUpdate, selfUpdate);
    }
}

public static class CourseExtensions
{
    public static async Task<Course?> FindCourseAsync(this DBWrapper db, string courseCode, CancellationToken token = default)
    {
        return await db.GetItemAsync(db => db.Courses, x => x.Code == courseCode, token);
    }

    public static async Task<bool> UpdateCourseAsync(this DBWrapper db, Course c, CancellationToken token = default)
    {
        return await db.PutOrUpdateItemAsync(db => db.Courses, c, x => x.Code == c.Code, token);
    }
}