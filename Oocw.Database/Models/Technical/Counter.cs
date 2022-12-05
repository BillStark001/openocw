using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oocw.Database.Models.Technical;
public class Counter
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; private set; }

    [BsonElement("dbName")]
    public string DBName { get; private set; } = null!;

    public int Sequel { get; private set; } = 1;

    public Counter(string dbName)
    {
        DBName = dbName;
        Sequel = 1;
    }
}
