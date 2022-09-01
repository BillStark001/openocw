using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Database;

namespace Oocw.Database.Models;

public class Relationship
{

    public enum Direction
    {
        None = 0b00, 
        User1 = 0b01, 
        User2 = 0b10,
        Both = User1 & User2,
    }

    public enum Status
    {
        Sent = 0, 
        Active = 1, 
        Inactive = 2, 
        Blocked = 3, 
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; private set; }

    [BsonElement(Definitions.KEY_USER1)]
    public int User1 { get; private set; } = -1;

    [BsonElement(Definitions.KEY_USER2)]
    public int User2 { get; private set; } = -1;

    [BsonElement(Definitions.KEY_LAST_UPD)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime LastUpdate { get; private set; } = DateTime.MinValue;

    [BsonElement(Definitions.KEY_TARGET)]
    [BsonRepresentation(BsonType.Int32)]
    public Direction Target { get; private set; } = Direction.None;

    [BsonElement(Definitions.KEY_STATUS)]
    [BsonRepresentation(BsonType.Int32)]
    public Status CurrentStatus { get; private set; } = Status.Sent;

    [BsonElement(Definitions.KEY_REASON)]
    public string? Reason { get; private set; } = null;

    // just to make it immutable
    public Relationship(string? id, int user1, int user2, DateTime lastUpdate, Direction target, Status currentStatus, string? reason)
    {
        Id = id;
        User1 = user1;
        User2 = user2;
        LastUpdate = lastUpdate;
        Target = target;
        CurrentStatus = currentStatus;
        Reason = reason;
    }
}

public static class RelationshipExtensions
{
    private static (int, int, bool) Sort2i(int a, int b)
    {
        if (a < b)
            return (a, b, false);
        else
            return (b, a, true);
    }

    private static Relationship? FindSimilarRelationInner(
        this DBWrapper db,
        int u1,
        int u2,
        IClientSessionHandle? session = null
        )
    {
        (u1, u2, _) = Sort2i(u1, u2);
        Relationship? res = session == null ?
            db.Relations.Find(x => x.User1 == u1 && x.User2 == u2).FirstOrDefault() :
            db.Relations.Find(session, x => x.User1 == u1 && x.User2 == u2).FirstOrDefault();
        return res;
    }

    public static Relationship? FindSimilarRelation(this DBWrapper db, int u1, int u2)
        => FindSimilarRelationInner(db, u1, u2);

    // TODO add methods
}
