using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oocw.Database.Models;

public class Faculty
{
    public class MetaInfo
    {

    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; private set; }

    [BsonElement(Definitions.KEY_META)]
    public MetaInfo Meta { get; private set; } = null!;


    [BsonElement(Definitions.KEY_ID)]
    public int Id { get; private set; }


    [BsonElement(Definitions.KEY_NAME)]
    public MultiLingualField Name { get; private set; } = null!;


    [BsonElement(Definitions.KEY_CONTACT)]
    public Dictionary<string, string> Contact { get; private set; } = null!;


}