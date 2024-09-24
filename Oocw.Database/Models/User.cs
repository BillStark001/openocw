using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Oocw.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class User(string loginName, string pwdEnc) : DataModel
{

    public enum Role
    {
        [BsonRepresentation(BsonType.String)]
        Guest = 0,

        [BsonRepresentation(BsonType.String)]
        Student = 1,

        [BsonRepresentation(BsonType.String)]
        Faculty = 2,

        [BsonRepresentation(BsonType.String)]
        Viewer = 4,

        [BsonRepresentation(BsonType.String)]
        Admin = 8,

    }


    public int Version { get; set; } = 0;



    // info
    public string LoginName { get; set; } = loginName;

    public string PasswordEncrypted { get; set; } = pwdEnc;

    [BsonRepresentation(BsonType.String)]
    public Role Group { get; set; } = Role.Guest;

    public HashSet<string> Departments { get; set; } = [];


    public MultiLingualField Name { get; set; } = new();

    public MultiLingualField PublicContact { get; set; } = new();

    public MultiLingualField InternalContact { get; set; } = new();

}

public static class UserExtensions
{
    public static async Task<User> CreateUserAsync(this OocwDatabase db, string uname, string pwdEnc)
    {
        using var session = await db.Client.StartSessionAsync();
        // check username conflict
        return await session.WithTransactionAsync(async (sess, ct) =>
        {
            if ((await db.Users.FindAsync(sess, x => x.LoginName == uname && !x.Deleted, cancellationToken: ct))
                .FirstOrDefault(cancellationToken: ct) != null)
                throw new UserNameConflictException(uname);

            // try insert & set id
            var id = await db.Users.InsertAsync(sess, new(uname, pwdEnc), cancellationToken: ct);

            return (await db.Users.FindAsync(sess, x => x.Id == id))
                .FirstOrDefault(cancellationToken: ct) ?? throw new DatabaseInternalException();
        });
    }

    public static User? QueryUser(this OocwDatabase db, string uname)
    {
        return db.Users.Find(x => x.LoginName == uname).FirstOrDefault();
    }

}

public class UserNameConflictException : Exception
{
    public UserNameConflictException(string? name = null) :
        base($"User Name Conflict: {name ?? ""}.")
    { }
}
