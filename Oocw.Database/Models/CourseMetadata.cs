using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Base;
using Oocw.Database.Models.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models;

public class CourseMetadata
{

    public const int CURRENT_VERSION = 1;
    public const string KEY_META = "__meta__";


    [BsonIgnoreIfNull]
    public string? OcwId { get; set; } = null;
    public bool Complete { get; set; } = false;
    public bool Dirty { get; set; } = true;


    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdateTime { get; set; } = DateTime.Now;
    public int Version { get; set; } = CURRENT_VERSION;


    [BsonIgnoreIfNull]
    public double? SeacrhScore { get; set; } = null;
    [BsonIgnoreIfNull]
    public string? SearchRecord { get; set; } = null;

}
