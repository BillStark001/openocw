using MongoDB.Driver;
using Oocw.Database.Models;
using Oocw.Database.Models.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oocw.Database;

public static class DatabaseExtensions
{
    /// <summary>
    /// Atomic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="col"></param>
    /// <param name="filter"></param>
    /// <param name="token"></param>
    /// <returns>The item if got, null if did not get</returns>
    public static async Task<T?> GetItemAsync<T>(
        this DBWrapper db,
        Func<DBWrapper, IMongoCollection<T>> col,
        Expression<Func<T, bool>> filter,
        CancellationToken token = default)
    {
        
        var cursor =
            db is DBSessionWrapper dbSess ?
            await col(dbSess).FindAsync(dbSess.Session, filter, cancellationToken: token) :
            await col(db).FindAsync(filter, cancellationToken: token);
        
        return await cursor.FirstOrDefaultAsync(token);
    }

    /// <summary>
    /// Could be non-atomic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="col"></param>
    /// <param name="c"></param>
    /// <param name="filter"></param>
    /// <param name="token"></param>
    /// <returns>True if update, false if put</returns>
    public static async Task<bool> PutOrUpdateItemAsync<T>(
        this DBWrapper db,
        Func<DBWrapper, IMongoCollection<T>> col,
        T c,
        Expression<Func<T, bool>> filter,
        CancellationToken token = default) 
        where T : IMergable<T>
    {
        var merge = c.GetMergeDefinition();
        var dbSess = db as DBSessionWrapper;
        UpdateResult def;
        UpdateOptions options = new() {
            IsUpsert = false, 
        };
        if (dbSess != null)
            def = await col(dbSess).UpdateOneAsync(dbSess.Session, filter, merge, options, cancellationToken: token);
        else
            def = await col(db).UpdateOneAsync(filter, merge, options, cancellationToken: token);

        if (def.MatchedCount > 0)
            return true;
        else
        {
            // update matching failed, insert
            if (dbSess != null)
                await col(dbSess).InsertOneAsync(dbSess.Session, c, cancellationToken: token);
            else
                await col(db).InsertOneAsync(c, cancellationToken: token);
            return false;
        }
    }
}
