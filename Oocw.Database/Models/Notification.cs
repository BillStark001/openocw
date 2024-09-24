

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class Notification(string userId, MultiLingualField message)
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SystemId { get; set; } = "";

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = userId;

    public bool Read { get; set; }

    public MultiLingualField Message { get; set; } = message;

}