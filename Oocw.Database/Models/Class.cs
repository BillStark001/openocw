using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;
using System.Xml.Linq;
using System.Linq.Expressions;
using Oocw.Base;

namespace Oocw.Database.Models;

public class Class : IMergable<Class>
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Metadata.KEY_META)]
    public Metadata Meta { get; set; } = null!;



    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdateTimeSyllabus { get; set; } = DateTime.MinValue;


    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdateTimeNotes { get; set; } = DateTime.MinValue;


    public string Code { get; set; } = "";

    public int Year { get; set; } = 1970;

    public string ClassName { get; set; } = "";

    public IEnumerable<int> Lecturers { get; set; } = new HashSet<int>();

    public int Format { get; set; }

    public int Quarter { get; set; }

    public IEnumerable<AddressInfo> Addresses { get; set; } = Enumerable.Empty<AddressInfo>();

    public string Language { get; set; } = "null";


    // public MultiLingualField Unit { get; set; } = new();


    public IEnumerable<int> Classes { get; set; } = new HashSet<int>();


    public IEnumerable<LectureInfo> Schedule { get; set; } = Enumerable.Empty<LectureInfo>();


    public IEnumerable<bool> Skills { get; set; } = new bool[5];
    public Dictionary<int, LectureInfo> Lectures { get; set; } = new();

    [BsonElement(Definitions.KEY_SYLLABUS)]
    public MultiVersionField<BsonDocument> Syllabus { get; set; } = new();

    
    public UpdateDefinition<P> GetMergeDefinition<P>(Expression<Func<P, Class>> expr)
    {
        
        var metaUpdate = Meta.GetMergeDefinition(expr.Combine(x => x.Meta));

        // var unitUpdate = Unit.GetMergeDefinition(expr.Combine(x => x.Unit));

        var selfUpdate = Builders<P>.Update
            .Set(expr.Combine(x => x.Year), Year)
            .Set(expr.Combine(x => x.ClassName), ClassName)
            .Set(expr.Combine(x => x.Lecturers), Lecturers)
            .Set(expr.Combine(x => x.Format), Format)
            .Set(expr.Combine(x => x.Quarter), Quarter)
            .Set(expr.Combine(x => x.Addresses), Addresses)
            .Set(expr.Combine(x => x.Language), Language)
            .Set(expr.Combine(x => x.Skills), Skills)
            .AddToSetEach(expr.Combine(x => x.Classes), Classes);

        // TODO try to merge
        var contentUpdate = Builders<P>.Update
            .Set(expr.Combine(x => x.Syllabus), Syllabus)
            .Set(expr.Combine(x => x.Lectures), Lectures);

        return Builders<P>.Update.Combine(metaUpdate, /*unitUpdate, */selfUpdate, contentUpdate);

    }
}

public static class ClassExtensions
{
    public static async Task<Class?> FindClassAsync(this DBWrapper db, string classCode, CancellationToken token = default)
    {
        return await db.GetItemAsync(db => db.Classes, x => x.Meta.OcwId == classCode, token);
    }

    public static async Task<bool> UpdateClassAsync(this DBWrapper db, Class c, CancellationToken token = default)
    {
        return await db.PutOrUpdateItemAsync(db => db.Classes, c, x => x.Meta.OcwId == c.Meta.OcwId, token);
    }
}