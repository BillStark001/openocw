

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class CourseSelection : DataModel
{

    public enum Status
    {
        Idle,
        Application,
        Approval,
        Dismissal,
        Closure,
    }

    public string StudentId { get; set; } = "";
    public string ClassInstanceId { get; set; } = "";

    [BsonRepresentation(BsonType.String)]
    public Status CurrentStatus { get; set; } = Status.Idle;

    public int Score { get; set; } = -1;
}