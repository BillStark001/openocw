using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Database;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Utils;

public static class DatabaseUtils
{
    public static string? TryGetTranslation(this object dict, string lang = "ja")
    {
        if (dict is BsonDocument)
            dict = ((BsonDocument)dict).ToDictionary();
        var d = (Dictionary<string, object>)dict;
        if (d.ContainsKey(lang))
            return (string?)d.GetValueOrDefault(lang);
        return (string?)d.Values.FirstOrDefault();
    }

}