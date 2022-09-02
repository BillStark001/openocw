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
    public static Course? GetCourseInfo(this DBWrapper db, string code)
    {
        return db.Courses.Find(x => x.Code == code).FirstOrDefault();
    }

    public static IEnumerable<(int, string)> GetFacultyNames(this DBWrapper db, IEnumerable<int> fs, string lang = "ja")
    {
        List<(int, string)> ans = new();
        Dictionary<int, string?> dt = new();
        foreach (var f in db.Faculties.Find(x => fs.Contains(x.Id)).ToList())
        {
            dt[f.Id] = f.Name.Translate(lang) ?? f.Name.ForceTranslate();
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