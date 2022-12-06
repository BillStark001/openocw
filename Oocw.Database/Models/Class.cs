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


    public string Code { get; set; } = null!;

    public int Year { get; set; } = 1970;

    public string ClassName { get; set; } = null!;

    public IEnumerable<int> Lecturers { get; set; } = null!;

    public int Format { get; set; }

    public int Quarter { get; set; }

    public IEnumerable<AddressInfo> Addresses { get; set; } = null!;

    public string Language { get; set; } = null!;


    [BsonElement(Definitions.KEY_UNIT)]
    public MultiLingualField Unit { get; set; } = null!;


    [BsonElement(Definitions.KEY_CLASSES)]
    public IEnumerable<int> Classes { get; set; } = null!;


    [BsonElement(Definitions.KEY_SYLLABUS)]
    public MultiVersionField<BsonDocument> Syllabus { get; set; } = null!;


    [BsonElement(Definitions.KEY_NOTES)]

    public MultiVersionField<BsonDocument> Notes { get; set; } = null!;
    
    public UpdateDefinition<P> GetMergeDefinition<P>(Func<P, Class> expr)
    {
        var metaUpdate = Meta.GetMergeDefinition<P>(x => expr(x).Meta);
        var unitUpdate = Unit.GetMergeDefinition<P>(x => expr(x).Unit);
        var selfUpdate = Builders<P>.Update
            .Set(x => expr(x).ClassName, ClassName)
            .Set(x => expr(x).Classes, Classes);
        // return Builders<P>.Update.Combine(metaUpdate, unitUpdate, selfUpdate);

        throw new NotImplementedException();
    }
}

public static class ClassExtensions
{
    public static async Task<Class?> FindClassAsync(this DBWrapper db, string classCode, CancellationToken token = default)
    {
        var cursor =
            db is DBSessionWrapper dbSess ?
            await dbSess.Classes.FindAsync(dbSess.Session, x => x.Meta.OcwId == classCode, cancellationToken: token) :
            await db.Classes.FindAsync(x => x.Meta.OcwId == classCode, cancellationToken: token);
        return await cursor.FirstOrDefaultAsync(token);
    }

    public static async Task UpdateClassAsync(this DBWrapper db, Class c, CancellationToken token = default)
    {
        var merge = c.GetMergeDefinition();
        UpdateResult def;
        if (db is DBSessionWrapper dbSess)
            def = await dbSess.Classes.UpdateOneAsync(dbSess.Session, x => x.Meta.OcwId == c.Meta.OcwId, merge, cancellationToken: token);
        else
            def = await db.Classes.UpdateOneAsync(x => x.Meta.OcwId == c.Meta.OcwId, merge, cancellationToken: token);

        if (def.MatchedCount > 0)
            return;
        else
        {
            // update matching failed, insert
            if (db is DBSessionWrapper dbSess2)
                await dbSess2.Classes.InsertOneAsync(dbSess2.Session, c, cancellationToken: token);
            else
                await db.Classes.InsertOneAsync(c, cancellationToken: token);
        }
    }
}