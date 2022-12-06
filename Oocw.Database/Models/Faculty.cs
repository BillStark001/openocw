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

public class Faculty : IMergable<Faculty>
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Metadata.KEY_META)]
    public Metadata Meta { get; set; } = new();


    public int Id { get; set; }
    public MultiLingualField Name { get; set; } = new();

    [BsonIgnoreIfNull]
    public Dictionary<string, string>? Contact { get; set; } = null;

    public UpdateDefinition<T> GetMergeDefinition<T>(Func<T, Faculty> expr)
    {
        var metaUpdate = Meta.GetMergeDefinition<T>(x => expr(x).Meta);
        var nameUpdate = Name.GetMergeDefinition<T>(x => expr(x).Name);
        var selfUpdate = Builders<T>.Update.Set(x => expr(x).Id, Id);
        if (Contact != null && Contact.Count > 0)
        {
            selfUpdate.Set(x => expr(x).Contact, Contact);
        }
        return Builders<T>.Update.Combine(selfUpdate, nameUpdate, metaUpdate);
    }
}

public static class FacultyExtensions
{
    public static async Task<Faculty?> FindFacultyAsync(this DBWrapper db, int id, CancellationToken token = default)
    {
        var cursor =
            db is DBSessionWrapper dbSess ?
            await dbSess.Faculties.FindAsync(dbSess.Session, x => x.Id == id, cancellationToken: token) :
            await db.Faculties.FindAsync(x => x.Id == id, cancellationToken: token);
        var res = await cursor.FirstOrDefaultAsync(cancellationToken: token);
        return res;
    }

    public static async Task<Faculty?> FindFacultyAsync(this DBWrapper db, string name, string lang = "ja", CancellationToken token = default)
    {
        var cursor =
            db is DBSessionWrapper dbSess ?
            await dbSess.Faculties.FindAsync(dbSess.Session, x => x.Name.Translate(lang) == name, cancellationToken: token) :
            await db.Faculties.FindAsync(x => x.Name.Translate(lang) == name, cancellationToken: token);
        var res = await cursor.FirstOrDefaultAsync(cancellationToken: token);
        return res;
    }

    public static async Task PutFacultyAsync(this DBWrapper db, Faculty f, CancellationToken token = default)
    {
        if (db is DBSessionWrapper dbSess)
            await dbSess.Faculties.InsertOneAsync(dbSess.Session, f, cancellationToken: token);
        else
            await db.Faculties.InsertOneAsync(f, cancellationToken: token);
    }

    public static async Task<UpdateResult> UpdateFacultyAsync(this DBWrapper db, Faculty f, CancellationToken token = default)
    {
        var merge = f.GetMergeDefinition();
        if (db is DBSessionWrapper dbSess)
            return await dbSess.Faculties.UpdateOneAsync(dbSess.Session, x => x.Id == f.Id, merge, cancellationToken: token);
        else
            return await db.Faculties.UpdateOneAsync(x => x.Id == f.Id, merge, cancellationToken: token);
    }
}