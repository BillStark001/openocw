

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oocw.Database.Models.Technical;



public class UpdateRequest
{


    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SystemId { get; set; } = "";


    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreateTime { get; set; }

    public string SenderId { get; set; } = "";

    public string TargetCollection { get; set; } = "";

    public string TargetObjectId { get; set; } = "";
    
    public BsonDocument Patch { get; set; } = [];


}