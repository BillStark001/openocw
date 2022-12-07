using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models;

public class LectureInfo
{
    public const int NUM_NONE = -1;
    public const int NUM_SUPP = -2;

    public int Number { get; set; } = NUM_NONE;
    public MultiLingualField Title { get; set; } = new();
    public MultiLingualField Detail { get; set; } = new();


    public int Type { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [BsonIgnoreIfNull]
    public DateTime? Date { get; set; }

    [BsonIgnoreIfNull]
    public IEnumerable<string>? Links { get; set; }
}
