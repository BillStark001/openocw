using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class Course
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Metadata.KEY_META)]
    public Metadata Meta { get; set; } = null!;


    public double AccessRank { get; set; }


    public string Code { get; set; } = "";

    public bool IsLink { get; set; }



    public MultiLingualField Name { get; set; } = new();

    public (double, double, double, int) Credit { get; set; }

    public MultiLingualField Unit { get; set; } = new();

    public IEnumerable<int> Classes { get; set; } = new List<int>();


}

public static class CourseExtensions
{
    public static async Task<Course?> FindCourseAsync(this DBWrapper db, string courseCode, CancellationToken token = default)
    {
        var cursor =
            db is DBSessionWrapper dbSess ?
            await dbSess.Courses.FindAsync(dbSess.Session, x => x.Code == courseCode, cancellationToken: token) :
            await db.Courses.FindAsync(x => x.Code == courseCode, cancellationToken: token);
        return await cursor.FirstOrDefaultAsync(token);
    }
}