using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models.Technical;

public class MultiVersionField<T> where T : class
{
    public const string VAL_VER_NONE = "none";

    public string Version { get; set; } = VAL_VER_NONE;

    public Dictionary<string, T> Items { get; set; } = new();

    public T? TryGetCurrentVersion()
    {
        T? ret = null;
        var succ = Items != null && Items.TryGetValue(Version, out ret);
        if (succ)
            return ret;
        return null;
    }
}