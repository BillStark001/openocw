using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using Oocw.Database.Models.Technical;
using System.Threading;

namespace Oocw.Database;

public sealed class DBSessionWrapper : DBWrapper, IDisposable
{
    private IClientSessionHandle _sess;

    public IClientSessionHandle Session => _sess;


    public DBSessionWrapper(IClientSessionHandle sess) : base(sess.Client)
    {
        _sess = sess;
    }

    public void Dispose()
    {
        _sess.Dispose();
    }

    public override int GetIncrementalId(string target)
    {
        var orig = _counters.FindOneAndUpdate(_sess, x => x.DBName == target, Builders<Counter>.Update.Inc(x => x.Sequel, 1));
        return orig.Sequel;
    }

    public override async Task<int> GetIncrementalIdAsync(string target)
    {
        var orig = await _counters.FindOneAndUpdateAsync(_sess, x => x.DBName == target, Builders<Counter>.Update.Inc(x => x.Sequel, 1));
        return orig.Sequel;
    }

    public override TResult UseTransaction<TResult>(TransactionCallback<TResult> callback, CancellationToken cToken = default)
    {
        throw new InvalidOperationException();
    }

    public override Task<TResult> UseTransactionAsync<TResult>(TransactionCallback<Task<TResult>> callback, CancellationToken cToken = default)
    {
        throw new InvalidOperationException();
    }
}

    