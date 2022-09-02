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
        public double AccessRank { get; private set; }


        [BsonElement(Definitions.KEY_SEARCH_REC)]
        public IEnumerable<string> SearchRecord { get; private set; } = null!;

        [BsonElement(Definitions.KEY_SEARCH_SCORE)]
        [BsonIgnoreIfNull]
        public double? SeacrhScore { get; private set; } = null!;


        [BsonElement(Definitions.KEY_UPD_TIME)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; private set; }

        [BsonElement(Definitions.KEY_DIRTY)]
        public bool Dirty { get; private set; }
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdRaw { get; private set; }

    [BsonElement(Definitions.KEY_META)]
    public MetaInfo Meta { get; private set; } = null!;


    [BsonElement(Definitions.KEY_CODE)]
    public string Code { get; private set; } = null!;


    [BsonElement(Definitions.KEY_ISLINK)]
    public bool IsLink { get; private set; }


    [BsonElement(Definitions.KEY_NAME)]
    public MultiLingualField Name { get; private set; } = null!;


    [BsonElement(Definitions.KEY_CREDIT)]
    public (double, double, double, int) Credit { get; private set; }


    [BsonElement(Definitions.KEY_UNIT)]
    public MultiLingualField Unit { get; private set; } = null!;


    [BsonElement(Definitions.KEY_CLASSES)]
    public IEnumerable<int> Classes { get; private set; } = null!;


}