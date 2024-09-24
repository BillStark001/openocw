
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;


public class CourseDiscussion : DataModel
{
    public string CourseId { get; set; } = "";
    public string SenderId { get; set; } = "";
    public string? ClassInstanceId { get; set; }
    public string? AssignmentId { get; set; }
    public string? ReplyId { get; set; }
    public bool IsPublic { get; set; }

    public MultiLingualField Content { get; set; } = new();
}