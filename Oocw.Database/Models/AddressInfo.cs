using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models;
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
        public string? Description { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement(Definitions.KEY_ADDR_DAY)]
        public int? Day { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement(Definitions.KEY_ADDR_START)]
        public int? Start { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement(Definitions.KEY_ADDR_END)]
        public int? End { get; set; }
    }

    [BsonElement(Definitions.KEY_ADDR_TYPE)]
    public AddressType Type { get; set; }

    [BsonElement(Definitions.KEY_ADDR_TIME)]
    public TimeInfo Time { get; set; } = null!;

    [BsonElement(Definitions.KEY_ADDR_LOCATION)]
    public string Location { get; set; } = null!;

}