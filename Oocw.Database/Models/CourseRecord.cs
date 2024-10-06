using MongoDB.Bson;
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

public class CourseRecord
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? SystemId { get; set; }

    [BsonElement]
    public string CourseId { get; set; } = "";

    public string Language { get; set; } = "en";

    [BsonElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateTime { get; set; }

    public bool Dirty { get; set; } = true;


    // search related

    [BsonIgnoreIfNull]
    public double SearchScore { get; set; }

    
    [BsonIgnoreIfNull]
    public string? ContentRecord { get; set; } = null;

    [BsonIgnoreIfNull]
    public string? CodeRecord { get; set; } = null;
    
    [BsonIgnoreIfNull]
    public string? InfoRecord { get; set; } = null;



    public double AccessRank { get; set; }

}
