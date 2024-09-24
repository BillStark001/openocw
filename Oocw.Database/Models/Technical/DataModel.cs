

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Oocw.Database.Models.Technical;

public class DataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? SystemId { get; set; }


    [BsonElement]
    public string Id { get; set; } = "";


    [BsonElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreateTime { get; set; }



    [BsonElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateTime { get; set; }


    [BsonElement]
    public bool Deleted { get; set; }


    public void SetCreateTime()
    {
        CreateTime = DateTime.UtcNow;
    }

    public void SetUpdateTime()
    {
        UpdateTime = DateTime.UtcNow;
    }

    public void MarkDeleted()
    {
        Deleted = true;
    }


}

public static class DataModelUtils
{

    const string PRINTABLE_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public static string GenerateRandomId(int length = 12)
    {

        var random = new Random();
        return new string(Enumerable.Repeat(PRINTABLE_CHARS, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static async Task<T?> FindByIdAsync<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        string? id,
        FindOptions<T, T>? options = null,
        CancellationToken cancellationToken = default
    ) where T : DataModel
    {
        if (string.IsNullOrWhiteSpace(id)) {
            return null;
        }
        var _f = Builders<T>.Filter;
        var filter = _f.And(
            _f.Eq(x => x.Id, id),
            _f.Eq(x => x.Deleted, false)
        );
        using var userCursor = session != null
            ? await collection.FindAsync(session, filter, options, cancellationToken)
            : await collection.FindAsync(filter, options, cancellationToken);
        var user = await userCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return user;
    }

    public static async Task<string> InsertAsync<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        T document,
        InsertOneOptions? options = null,
        int maxRetry = 3,
        CancellationToken cancellationToken = default
    ) where T : DataModel
    {
        int currentRetry = 0;
        while (true)
        {
            try
            {
                document.Id = GenerateRandomId();
                if (session == null)
                    await collection.InsertOneAsync(document, options, cancellationToken);
                else
                    await collection.InsertOneAsync(session, document, options, cancellationToken);
                return document.Id;
            }
            catch (MongoWriteException ex) when (
                ex.WriteError.Category == ServerErrorCategory.DuplicateKey
                && currentRetry < maxRetry
                )
            {
                ++currentRetry;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static async Task DeleteAsync<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle session,
        T document,
        UpdateOptions? options = null,
        CancellationToken cancellationToken = default
    )  where T : DataModel
    {
        var res = await collection.UpdateOneAsync(
            session,
            x => x.Id == document.Id,
            Builders<T>.Update
                .Set(x => x.Deleted, true)
                .Set(x => x.UpdateTime, DateTime.UtcNow),
            options,
            cancellationToken
        );
        if (res?.ModifiedCount != 1) {
            throw new DatabaseInternalException();
        }
    }
}
