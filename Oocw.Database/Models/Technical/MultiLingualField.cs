using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Base;

namespace Oocw.Database.Models.Technical;

[BsonIgnoreExtraElements]
public class MultiLingualField : Dictionary<string, string>
{
    public new string? this[string languageCode]
    {
        get => GetTranslation(languageCode);
        set => SetTranslation(languageCode, value);
    }

    public string? GetTranslation(string languageCode)
    {
        if (TryGetValue(languageCode, out string? translation))
        {
            return translation;
        }

        // find the closest
        var closestMatch = Keys.FirstOrDefault(k => k.StartsWith(languageCode) || languageCode.StartsWith(k));
        if (closestMatch != null)
        {
            return base[closestMatch];
        }

        // find english
        if (TryGetValue("en", out translation) || TryGetValue("en-US", out translation) || TryGetValue("en-GB", out translation))
        {
            return translation;
        }

        // find the first available
        return Values.FirstOrDefault();
    }

    public void SetTranslation(string languageCode, string? value) {
        if (value != null) {
            base[languageCode] = value;
        }
        else if (ContainsKey(languageCode))
        {
            Remove(languageCode);
        }
    }

    // shortcuts of frequent-used languages
    public string? Zh
    {
        get => GetTranslation("zh-CN") ?? GetTranslation("zh") ?? GetTranslation("zh-TW");
        set => SetTranslation("zh-CN", value);
    }

    public string? En
    {
        get => GetTranslation("en-US") ?? GetTranslation("en") ?? GetTranslation("en-GB");
        set => SetTranslation("en-US", value);
    }

    public string? Ja
    {
        get => GetTranslation("ja-JP") ?? GetTranslation("ja");
        set => SetTranslation("ja-JP", value);
    }

    public new bool ContainsKey(string languageCode)
    {
        return base.ContainsKey(languageCode);
    }

    public IEnumerable<string> AvailableLanguages => Keys;
}