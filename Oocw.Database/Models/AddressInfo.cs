using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Database.Models;
public class AddressInfo
{

    public const int VAL_TYPE_NONE = 0;
    public const int VAL_TYPE_NORMAL = 1;
    public const int VAL_TYPE_SPECIAL = 2;
    public const int VAL_TYPE_UNKNOWN = 3;
    
    public enum AddressType
    {
        None = VAL_TYPE_NONE,
        Normal = VAL_TYPE_NORMAL,
        Special = VAL_TYPE_SPECIAL,
        Unknown = VAL_TYPE_UNKNOWN,
    }

    public class TimeInfo
    {
        [BsonIgnoreIfNull]
        [BsonElement]
        public string? Description { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement]
        public int? Day { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement]
        public int? Start { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement]
        public int? End { get; set; }
    }

    public AddressType Type { get; set; }

    public int Year { get; set; }

    public int Term { get; set; }

    public TimeInfo Time { get; set; } = new();

    public string Location { get; set; } = "";

}