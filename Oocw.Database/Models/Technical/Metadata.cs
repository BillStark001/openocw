using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models.Technical;

public class Metadata : IMergable<Metadata>
{

    public const int CURRENT_VERSION = 1;
    public const string KEY_META = "__meta__";


    [BsonIgnoreIfNull]
    public string? OcwId { get; set; } = null;
    public bool Complete { get; set; } = false;
    public bool Dirty { get; set; } = true;


    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdateTime { get; set; } = DateTime.Now;
    public int Version { get; set; } = CURRENT_VERSION;


    [BsonIgnoreIfNull]
    public double? SeacrhScore { get; set; } = null;
    [BsonIgnoreIfNull]
    public string? SearchRecord { get; set; } = null;

    public UpdateDefinition<P> GetMergeDefinition<P>(Func<P, Metadata> expr)
    {
        return Builders<P>.Update
            .Set(x => expr(x).Dirty, true)
            .Set(x => expr(x).UpdateTime, DateTime.Now);
    }
}
