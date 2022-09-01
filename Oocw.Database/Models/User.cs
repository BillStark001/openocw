using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Oocw.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Oocw.Database.Models.Relationship;
using MongoDB.Driver;

namespace Oocw.Database.Models;

public class User
{

    public enum UserGroup
    {
        [BsonRepresentation(BsonType.String)]
        User = 0,

        [BsonRepresentation(BsonType.String)]
        Guest = 1,

        [BsonRepresentation(BsonType.String)]
        Admin = 2,

    }

    [BsonIgnore]
    private DBWrapper? _db = null;

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; private set; }

    [BsonElement(Definitions.KEY_VERSION)]
    public int Version { get; private set; } = 1; // version 2


    [BsonElement(Definitions.KEY_ID)]
    public int Id { get; private set; } = -1;

    [BsonElement(Definitions.KEY_SEL_PERS)]
    public int SelectedPersona { get; private set; } = -1;


    // info

    [BsonElement(Definitions.KEY_USERNAME)]
    public string UserName { get; private set; } = null!;

    [BsonElement(Definitions.KEY_PASSWORD)]
    public string PasswordEncrypted { get; private set; } = null!;


    // flags

    [BsonElement(Definitions.KEY_DELETED)]
    public bool Deleted { get; private set; } = false;

    [BsonElement(Definitions.KEY_REFRESH_TIME)]
    public DateTime RefreshTime { get; private set; } = DateTime.MinValue;

    [BsonElement(Definitions.KEY_GROUP)]
    [BsonRepresentation(BsonType.String)]
    public UserGroup Group { get; private set; } = UserGroup.User; // version 2

    // metas that won't store
    [BsonIgnore]
    public IEnumerable<int> Personas { get; private set; } = Enumerable.Empty<int>();


    // ctor

    public User(DBWrapper? db, string? idRaw, int version, int id, int selectedPersona, string userName, string passwordEncrypted, bool deleted, UserGroup group, IEnumerable<int> personas)
    {
        _db = db;
        IdRaw = idRaw;
        Version = version;
        Id = id;
        SelectedPersona = selectedPersona;
        UserName = userName;
        PasswordEncrypted = passwordEncrypted;
        Deleted = deleted;
        Group = group;
        Personas = personas;
    }

    public User(string uname, string pwdEnc)
    {
        UserName = uname;
        PasswordEncrypted = pwdEnc;
    }

    internal User SetDB(DBWrapper db)
    {
        if (_db != null || db is DBSessionWrapper)
            throw new DatabaseInternalException();
        _db = db;
        return this;
    }

    public bool Update()
    {
        var u = _db!.Users.Find(x => x.IdRaw == IdRaw).FirstOrDefault();
        if (u == null)
            return false;
        // update infos
        // maybe we should use reflection to accelerate?
        Version = u.Version;
        Id = u.Id;
        SelectedPersona = u.SelectedPersona;
        UserName = u.UserName;
        PasswordEncrypted = u.PasswordEncrypted;
        Deleted = u.Deleted;
        Group = u.Group;
        return true;
    }

    // TODO update meta

    public bool MarkDeleted()
    {
        var res = _db!.Users.UpdateOne(x => x.IdRaw == IdRaw, Builders<User>.Update.Set(x => x.Deleted, true));
        var ret = res.ModifiedCount > 0;
        if (ret)
            Deleted = true;
        return ret;
    }

    // TODO persona related
}

public static class UserExtensions
{
    public static User? FindUser(this DBWrapper db, int id)
    {
        return db.Users.Find(x => x.Id == id).FirstOrDefault()?.SetDB(db);
    }

    public static User PutUser(this DBWrapper db, string uname, string pwdEnc)
    {
        return db.UseTransaction((db, ct) =>
        {
            // check conflict
            if (db.Users.Find(db.Session, x => x.UserName == uname && !x.Deleted).FirstOrDefault() != null)
                throw new UserNameConflictException(uname);

            // try insert
            db.Users.InsertOne(db.Session, new(uname, pwdEnc));
            var user = db.Users.Find(db.Session, x => x.UserName == uname).FirstOrDefault() ?? throw new DatabaseInternalException();

            // set id
            var id = db.GetUserIncrementalId();
            var res = db.Users.UpdateOne(
                db.Session, x => x.IdRaw == user.IdRaw,
                Builders<User>.Update.Set(x => x.Id, id).Set(x => x.RefreshTime, DateTime.UtcNow)
                );
            if (res.ModifiedCount == 0)
                throw new DatabaseInternalException();

            return db.Users.Find(db.Session, x => x.IdRaw == user.IdRaw).FirstOrDefault() ?? throw new DatabaseInternalException();
        })!.SetDB(db);
    }

    public static User? QueryUser(this DBWrapper db, string uname)
    {
        return db.Users.Find(x => x.UserName == uname).FirstOrDefault()?.SetDB(db);
    }

}

public class UserNameConflictException: Exception
{
    public UserNameConflictException(string? name = null) :
        base($"User Name Conflict: {name ?? ""}.")
    { }
}
