using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models.Technical;

public interface IMergable<T>
{
    public UpdateDefinition<P> GetMergeDefinition<P>(Func<P, T> expr);
}


public static class MergableExtensions
{
    public static UpdateDefinition<T> GetMergeDefinition<T>(this IMergable<T> mergable) => mergable.GetMergeDefinition<T>(x => x);
}