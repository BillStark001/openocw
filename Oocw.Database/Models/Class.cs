using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class Class
{


    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Metadata.KEY_META)]
    public Metadata Meta { get; set; } = null!;



    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdateTimeSyllabus { get; set; }


    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdateTimeNotes { get; set; }


    public string Code { get; set; } = null!;

    public int Year { get; set; } = 1970;

    public string ClassName { get; set; } = null!;

    public IEnumerable<int> Lecturers { get; set; } = null!;

    public int Format { get; set; }

    public int Quarter { get; set; }

    public IEnumerable<AddressInfo> Addresses { get; set; } = null!;

    public string Language { get; set; } = null!;


    [BsonElement(Definitions.KEY_UNIT)]
    public MultiLingualField Unit { get; set; } = null!;


    [BsonElement(Definitions.KEY_CLASSES)]
    public IEnumerable<int> Classes { get; set; } = null!;


    [BsonElement(Definitions.KEY_SYLLABUS)]
    public MultiVersionField<BsonDocument> Syllabus { get; set; } = null!;


    [BsonElement(Definitions.KEY_NOTES)]

    public MultiVersionField<BsonDocument> Notes { get; set; } = null!;
}