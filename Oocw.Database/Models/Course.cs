using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oocw.Database.Models;

public class Course
{
    public class MetaInfo
    {
        [BsonElement(Definitions.KEY_ACCESS_RANK)]
        public double AccessRank { get; set; }


        [BsonElement(Definitions.KEY_SEARCH_REC)]
        public IEnumerable<string> SearchRecord { get; set; } = null!;

        [BsonElement(Definitions.KEY_SEARCH_SCORE)]
        [BsonIgnoreIfNull]
        public double? SeacrhScore { get; set; } = null!;


        [BsonElement(Definitions.KEY_UPD_TIME)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        [BsonElement(Definitions.KEY_DIRTY)]
        public bool Dirty { get; set; }
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; set; }

    [BsonElement(Definitions.KEY_META)]
    public MetaInfo Meta { get; set; } = null!;


    [BsonElement(Definitions.KEY_CODE)]
    public string Code { get; set; } = null!;


    [BsonElement(Definitions.KEY_ISLINK)]
    public bool IsLink { get; set; }


    [BsonElement(Definitions.KEY_NAME)]
    public MultiLingualField Name { get; set; } = null!;


    [BsonElement(Definitions.KEY_CREDIT)]
    public (double, double, double, int) Credit { get; set; }


    [BsonElement(Definitions.KEY_UNIT)]
    public MultiLingualField Unit { get; set; } = null!;


    [BsonElement(Definitions.KEY_CLASSES)]
    public IEnumerable<int> Classes { get; set; } = null!;


}