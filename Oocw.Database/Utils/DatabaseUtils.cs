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
    public static Course? GetCourseInfo(this OocwDatabase db, string code)
    {
        return db.Courses.Find(x => x.CourseCode == code).FirstOrDefault();
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

    public static async Task CreateTextIndexIfNotExistsAsync(IMongoDatabase database, string collectionName, string indexName)
{
    var collection = database.GetCollection<BsonDocument>(collectionName);
    
    // 获取现有索引列表
    using (var cursor = await collection.Indexes.ListAsync())
    {
        var indexes = await cursor.ToListAsync();
        
        // 检查是否已存在具有指定名称的索引
        var indexExists = indexes.Any(index => index["name"] == indexName);
        
        if (!indexExists)
        {
            // 如果索引不存在，则创建它
            var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Text("$**");
            var indexOptions = new CreateIndexOptions { Name = indexName };
            var model = new CreateIndexModel<BsonDocument>(indexKeysDefinition, indexOptions);
            
            await collection.Indexes.CreateOneAsync(model);
            Console.WriteLine($"Created text index '{indexName}' on collection '{collectionName}'");
        }
        else
        {
            Console.WriteLine($"Text index '{indexName}' already exists on collection '{collectionName}'");
        }
    }
}
}