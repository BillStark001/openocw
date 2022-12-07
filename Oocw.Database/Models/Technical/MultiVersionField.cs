using System.Collections.Generic;

namespace Oocw.Database.Models.Technical;

public class MultiVersionField<T> where T : class
{
    public const string VAL_VER_NONE = "none";

    public string Version { get; set; } = VAL_VER_NONE;

    public Dictionary<string, T> Items { get; set; } = new();

    public T? GetItem() => GetItem(Version);

    public void PutItem(T item, string version, bool setVersion = true)
    {
        Items[version] = item;
        if (setVersion)
            Version = version;
    }

    public T? GetItem(string version)
    {
        return Items.TryGetValue(version, out var ret) ? ret : null;
    }
}