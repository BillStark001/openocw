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
        public int OcwId { get; private set; }

        [BsonElement(Definitions.KEY_COMPLETE)]
        public bool Complete { get; private set; }

        [BsonElement(Definitions.KEY_DIRTY)]
        public bool Dirty { get; private set; }


        [BsonElement(Definitions.KEY_UPD_TIME)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; private set; }

        [BsonElement(Definitions.KEY_UPD_TIME_SYL)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTimeSyllabus { get; private set; }

        [BsonElement(Definitions.KEY_UPD_TIME_NOTES)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTimeNotes { get; private set; }


        [BsonElement(Definitions.KEY_SEARCH_REC)]
        public IEnumerable<string> SearchRecord { get; private set; } = null!;

        [BsonElement(Definitions.KEY_SEARCH_SCORE)]
        [BsonIgnoreIfNull]
        public double? SeacrhScore { get; private set; } = null!;
    }

    public class AddressInfo
    {
        public enum AddressType
        {
            None = Definitions.VAL_TYPE_NONE, 
            Normal = Definitions.VAL_TYPE_NORMAL, 
            Special = Definitions.VAL_TYPE_SPECIAL, 
            Unknown = Definitions.VAL_TYPE_UNKNOWN, 
        }

        public class TimeInfo
        {
            [BsonIgnoreIfNull]
            [BsonElement(Definitions.KEY_ADDR_DESC)]
            public string? Description { get; private set; }

            [BsonIgnoreIfNull]
            [BsonElement(Definitions.KEY_ADDR_DAY)]
            public int? Day { get; private set; }

            [BsonIgnoreIfNull]
            [BsonElement(Definitions.KEY_ADDR_START)]
            public int? Start { get; private set; }

            [BsonIgnoreIfNull]
            [BsonElement(Definitions.KEY_ADDR_END)]
            public int? End { get; private set; }
        }

        [BsonElement(Definitions.KEY_ADDR_TYPE)]
        public AddressType Type { get; private set; }

        [BsonElement(Definitions.KEY_ADDR_TIME)]
        public TimeInfo Time { get; private set; } = null!;

        [BsonElement(Definitions.KEY_ADDR_LOCATION)]
        public string Location { get; private set; } = null!;

    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; private set; }

    [BsonElement(Definitions.KEY_META)]
    public MetaInfo Meta { get; private set; } = null!;



    [BsonElement(Definitions.KEY_CODE)]
    public string Code { get; private set; } = null!;


    [BsonElement(Definitions.KEY_YEAR)]
    public int Year { get; private set; } = 1970;


    [BsonElement(Definitions.KEY_CLASS_NAME)]
    public string ClassName { get; private set; } = null!;


    [BsonElement(Definitions.KEY_LECTURERS)]
    public IEnumerable<int> Lecturers { get; private set; } = null!;


    [BsonElement(Definitions.KEY_FORMAT)]
    public int Format { get; private set; }


    [BsonElement(Definitions.KEY_QUARTER)]
    public int Quarter { get; private set; }


    [BsonElement(Definitions.KEY_ADDRESS)]
    public IEnumerable<AddressInfo> Addresses { get; private set; } = null!;

    [BsonElement(Definitions.KEY_LANGUAGE)]
    public string Language { get; private set; } = null!;


    [BsonElement(Definitions.KEY_UNIT)]
    public MultiLingualField Unit { get; private set; } = null!;


    [BsonElement(Definitions.KEY_CLASSES)]
    public IEnumerable<int> Classes { get; private set; } = null!;


    [BsonElement(Definitions.KEY_SYLLABUS)]
    public MultiVersionField<BsonDocument> Syllabus { get; private set; } = null!;


    [BsonElement(Definitions.KEY_NOTES)]

    public MultiVersionField<BsonDocument> Notes { get; private set; } = null!;
}