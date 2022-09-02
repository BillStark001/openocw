using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Oocw.Database.Models;
public class MultiLingualField
{
    [BsonElement(Definitions.KEY_LANG_KEY)]
    public string? Key { get; set; }


    [BsonElement("ja")]
    public string? Ja { get; set; }

    [BsonElement("en")]
    public string? En { get; set; }

    [BsonElement("zh")]
    public string? Zh { get; set; }

    [BsonElement("ko")]
    public string? Ko { get; set; }


    public string? Translate(string target="ja")
    {
        string? ans = null;
        target = target.ToLower();
        if (target == Definitions.KEY_LANG_KEY)
            ans = Key;
        if (target.StartsWith("en"))
            ans = En;
        else if (target.StartsWith("ja"))
            ans = Ja;
        else if (target.StartsWith("zh"))
            ans = Zh;
        else if (target.StartsWith("ko"))
            ans = Ko;
        return ans;
    }

    public string ForceTranslate()
    {
        return Ja ?? En ?? Zh ?? Ko ?? Key ?? "";
    }

    [BsonIgnore]
    private static Dictionary<string, PropertyInfo> Fields;
    static MultiLingualField()
    {
        Fields = new();
        foreach (var p in typeof(MultiLingualField).GetProperties())
        {
            var key = p.GetCustomAttributes()
                .Where(a => a.GetType() == typeof(BsonElementAttribute))
                .Select(x => ((BsonElementAttribute)x).ElementName)
                .FirstOrDefault((string?) null);
            if (!string.IsNullOrEmpty(key))
                Fields[key] = p;
        }
    }


    public Dictionary<string, string> AsDictionary(bool preserveNull = false)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (var (key, prop) in Fields)
        {
            var val = (string?) prop.GetGetMethod()!.Invoke(this, Array.Empty<object?>());
            if (val != null) // empty string is included
                dic[key] = val;
            else if (preserveNull)
                dic[key] = "";
        }
        return dic;
    }
}
