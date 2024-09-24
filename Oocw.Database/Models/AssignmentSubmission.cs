

using System.Collections.Generic;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;


public class AssignmentSubmission : DataModel
{
    public enum ContentType
    {
        Text,
        File,
        Media,
    }

    public class Content
    {
        public ContentType Type { get; set; } = ContentType.Text;

        public string Text { get; set; } = "";

    }

    public string ClassInstanceId { get; set; } = "";
    public string ContentId { get; set; } = "";
    public string StudentId { get; set; } = "";

    public List<Content> Contents { get; set; } = [];

}