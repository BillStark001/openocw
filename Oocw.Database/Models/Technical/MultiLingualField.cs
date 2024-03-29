﻿using System;
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
public class MultiLingualField : IMergable<MultiLingualField>
{

    public const string KEY_LANG_KEY = "__key__";

    [BsonElement(KEY_LANG_KEY)]
    public string Key { get; set; } = "null";


    [BsonElement("ja")]
    [BsonIgnoreIfNull]
    public string? Ja { get; set; }

    [BsonElement("en")]
    [BsonIgnoreIfNull]
    public string? En { get; set; }

    [BsonElement("zh")]
    [BsonIgnoreIfNull]
    public string? Zh { get; set; }

    [BsonElement("ko")]
    [BsonIgnoreIfNull]
    public string? Ko { get; set; }


    public static Expression<Func<T, bool>> TranslateOnFilter<T>(Expression<Func<T, MultiLingualField>> expr, Expression<Func<string?, bool>> comparer, string target = "ja")
    {

        var middleExpr = Expressions.TryGetValue(target.ToLower(), out var middleExpr2) ? middleExpr2 : Expressions[KEY_LANG_KEY];
        return expr.Combine(middleExpr).Combine(comparer);
    }

    public string? Translate(string target = "ja")
    {
        string? ans = null;
        target = target.ToLower();
        if (target == KEY_LANG_KEY)
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
    [BsonIgnore]
    private static Dictionary<string, Expression<Func<MultiLingualField, string?>>> Expressions;
    static MultiLingualField()
    {
        Fields = new();
        foreach (var p in typeof(MultiLingualField).GetProperties())
        {
            var key = p.GetCustomAttributes()
                .Where(a => a.GetType() == typeof(BsonElementAttribute))
                .Select(x => ((BsonElementAttribute)x).ElementName)
                .FirstOrDefault((string?)null);
            if (!string.IsNullOrEmpty(key) && key != KEY_LANG_KEY)
                Fields[key] = p;
        }

        Expressions = new()
        {
            [KEY_LANG_KEY] = x => x.Key, 
            ["ja"] = x => x.Ja,
            ["en"] = x => x.En,
            ["zh"] = x => x.Zh,
            ["ko"] = x => x.Ko
        };
        
    }

    public void Update(string? value, string lang = "ja")
    {
        Fields[lang].SetValue(this, value);
    }

    public Dictionary<string, string> AsDictionary(bool preserveNull = false)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (var (key, prop) in Fields)
        {
            var val = (string?)prop.GetGetMethod()!.Invoke(this, Array.Empty<object?>());
            if (val != null) // empty string is included
                dic[key] = val;
            else if (preserveNull)
                dic[key] = "";
        }
        return dic;
    }

    public UpdateDefinition<P> GetMergeDefinition<P>(Expression<Func<P, MultiLingualField>> expr)
    {
        var ret = Builders<P>.Update
            .Set(ExpressionUtils.Combine(expr, x => x.Key), Key);

        foreach (var (k, field) in Fields)
        {
            var val = field.GetValue(this) as string;
            if (val != null) // intentional, even if empty, consider there is a value
                ret = ret.Set(ExpressionUtils.Combine(expr, Expressions[k]), val);
        }
        return ret;
    }
}
