using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models;

public class MultiVersionField<T> where T: class
{
    [BsonElement(Definitions.KEY_VERSION)]
    public string Version { get; set; } = Definitions.VAL_VER_NONE;

    [BsonElement(Definitions.KEY_ITEMS)]
    public IDictionary<string, T> Items { get; set; } = null!;

    public T? TryGetCurrentVersion()
    {
        T? ret = null;
        var succ = Items != null && Items.TryGetValue(Version, out ret);
        if (succ)
            return ret;
        return null;
    }
}