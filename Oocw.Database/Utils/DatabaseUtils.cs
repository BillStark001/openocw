using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Utils;

public static class DatabaseUtils
{
    public static BsonDocument? GetCourseInfo(this DBWrapper db, string classId)
    {
        return db.Courses.Find(Builders<BsonDocument>.Filter.Eq(Definitions.KEY_CODE, classId)).FirstOrDefault();
    }

    public static IEnumerable<(int, string)> GetFacultyNames(this DBWrapper db, IEnumerable<int> fs, string lang = "ja")
    {
        List<(int, string)> ans = new();
        var qs = Builders<BsonDocument>.Filter.In(Definitions.KEY_ID, fs);
        Dictionary<int, string?> dt = new();
        foreach (var f in db.Faculties.Find(qs).ToList())
        {
            f.TryGetElement(Definitions.KEY_NAME, out var nameRaw);
            f.TryGetElement(Definitions.KEY_ID, out var idRaw);
            dt[(int)idRaw.Value] = nameRaw.Value.TryGetTranslation(lang);
        }
        foreach (var id in fs)
        {
            ans.Add((id, dt[id] ?? $"Not Found ({id})"));
        }
        return ans;
    }

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