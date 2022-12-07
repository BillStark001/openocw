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

    public UpdateDefinition<T> GetMergeDefinition<T>(Expression<Func<T, Faculty>> expr)
    {
        var metaUpdate = Meta.GetMergeDefinition<T>(ExpressionUtils.Combine(expr, x => x.Meta));
        var nameUpdate = Name.GetMergeDefinition<T>(ExpressionUtils.Combine(expr, x => x.Name));
        var selfUpdate = Builders<T>.Update.Set(ExpressionUtils.Combine(expr, x => x.Id), Id);
        if (Contact != null && Contact.Count > 0)
        {
            // TODO try merging?
            selfUpdate.Set(ExpressionUtils.Combine(expr, x => x.Contact), Contact);
        }
        return Builders<T>.Update.Combine(selfUpdate, nameUpdate, metaUpdate);
    }
}

public static class FacultyExtensions
{
    public static async Task<Faculty?> FindFacultyAsync(this DBWrapper db, int id, CancellationToken token = default)
    {
        return await db.GetItemAsync(db => db.Faculties, x => x.Id == id, token);
    }

    public static async Task<Faculty?> FindFacultyAsync(this DBWrapper db, string name, string lang = "ja", CancellationToken token = default)
    {
        var filter = MultiLingualField.TranslateOnFilter<Faculty>(x => x.Name, x => x == name, lang);
        return await db.GetItemAsync(db => db.Faculties, filter, token);
    }

    public static async Task<bool> UpdateFacultyAsync(this DBWrapper db, Faculty f, CancellationToken token = default)
    {
        return await db.PutOrUpdateItemAsync(db => db.Faculties, f, x => x.Id == f.Id, token);
    }
}