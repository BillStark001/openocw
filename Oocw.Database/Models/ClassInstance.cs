

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class ClassInstance : DataModel
{

    public enum ContentType
    {
        Text,
        File,
        Media,
        Assignment,
    }

    public class Content
    {
        public string Id {get; set;} = "";
        public int LectureNumber { get; set; }
        public DateTime? LectureDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ContentType Type { get; set; } = ContentType.Text;

        public string Text { get; set; } = "";
        public bool IsPublic { get; set; }

    }

    public string ClassId { get; set; } = "";
    public List<string> Lecturers { get; set; } = [];

    public AddressInfo Address { get; set; } = new();

    public List<Content> Contents { get; set; } = [];

}