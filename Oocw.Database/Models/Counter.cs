using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database;

namespace Oocw.Database.Models;
public class Counter
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; private set; }

    [BsonElement(Definitions.KEY_DB_NAME)]
    public string DBName { get; private set; } = null!;

    [BsonElement(Definitions.KEY_SEQ)]
    public int Sequel { get; private set; } = 1;

    public Counter(string dbName)
    {
        DBName = dbName;
        Sequel = 1;
    }
}
