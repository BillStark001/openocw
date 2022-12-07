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
    public DateTime UpdateTimeSyllabus { get; set; }


    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdateTimeNotes { get; set; }


    public string Code { get; set; } = "";

    public int Year { get; set; } = 1970;

    public string ClassName { get; set; } = "";

    public IEnumerable<int> Lecturers { get; set; } = new HashSet<int>();

    public int Format { get; set; }

    public int Quarter { get; set; }

    public IEnumerable<AddressInfo> Addresses { get; set; } = Enumerable.Empty<AddressInfo>();

    public string Language { get; set; } = "null";


    [BsonElement(Definitions.KEY_UNIT)]
    public MultiLingualField Unit { get; set; } = new();


    [BsonElement(Definitions.KEY_CLASSES)]
    public IEnumerable<int> Classes { get; set; } = new HashSet<int>();


    [BsonElement(Definitions.KEY_SYLLABUS)]
    public MultiVersionField<BsonDocument> Syllabus { get; set; } = new();


    [BsonElement(Definitions.KEY_NOTES)]

    public MultiVersionField<BsonDocument> Notes { get; set; } = new();
    
    public UpdateDefinition<P> GetMergeDefinition<P>(Expression<Func<P, Class>> expr)
    {
        
        var metaUpdate = Meta.GetMergeDefinition<P>(ExpressionUtils.Combine(expr, x => x.Meta));
        var unitUpdate = Unit.GetMergeDefinition<P>(ExpressionUtils.Combine(expr, x => x.Unit));
        var selfUpdate = Builders<P>.Update
            .Set(ExpressionUtils.Combine(expr, x => x.Year), Year)
            .Set(ExpressionUtils.Combine(expr, x => x.ClassName), ClassName)
            .AddToSetEach(ExpressionUtils.Combine(expr, x => x.Lecturers), Lecturers)
            .BitwiseAnd(ExpressionUtils.Combine(expr, x => x.Format), Format)
            .BitwiseAnd(ExpressionUtils.Combine(expr, x => x.Quarter), Quarter)
            .AddToSetEach(ExpressionUtils.Combine(expr, x => x.Addresses), Addresses)
            .Set(ExpressionUtils.Combine(expr, x => x.Language), Language)
            .AddToSetEach(ExpressionUtils.Combine(expr, x => x.Classes), Classes);
        // return Builders<P>.Update.Combine(metaUpdate, unitUpdate, selfUpdate);

        throw new NotImplementedException();
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