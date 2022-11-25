using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oocw.Database.Models;

public class Class
{

    public class MetaInfo
    {
        [BsonElement(Definitions.KEY_OCW_ID)]
        public int OcwId { get; set; }

        [BsonElement(Definitions.KEY_COMPLETE)]
        public bool Complete { get; set; }

        [BsonElement(Definitions.KEY_DIRTY)]
        public bool Dirty { get; set; }


        [BsonElement(Definitions.KEY_UPD_TIME)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        [BsonElement(Definitions.KEY_UPD_TIME_SYL)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTimeSyllabus { get; set; }

        [BsonElement(Definitions.KEY_UPD_TIME_NOTES)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTimeNotes { get; set; }


        [BsonElement(Definitions.KEY_SEARCH_REC)]
        public IEnumerable<string> SearchRecord { get; set; } = null!;

        [BsonElement(Definitions.KEY_SEARCH_SCORE)]
        [BsonIgnoreIfNull]
        public double? SeacrhScore { get; set; } = null!;
    }

    

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Definitions.KEY_META)]
    public MetaInfo Meta { get; set; } = null!;



    [BsonElement(Definitions.KEY_CODE)]
    public string Code { get; set; } = null!;


    [BsonElement(Definitions.KEY_YEAR)]
    public int Year { get; set; } = 1970;


    [BsonElement(Definitions.KEY_CLASS_NAME)]
    public string ClassName { get; set; } = null!;


    [BsonElement(Definitions.KEY_LECTURERS)]
    public IEnumerable<int> Lecturers { get; set; } = null!;


    [BsonElement(Definitions.KEY_FORMAT)]
    public int Format { get; set; }


    [BsonElement(Definitions.KEY_QUARTER)]
    public int Quarter { get; set; }


    [BsonElement(Definitions.KEY_ADDRESS)]
    public IEnumerable<AddressInfo> Addresses { get; set; } = null!;

    [BsonElement(Definitions.KEY_LANGUAGE)]
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