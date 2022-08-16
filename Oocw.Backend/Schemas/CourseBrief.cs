using MongoDB.Bson;
using Oocw.Backend.Database;

namespace Oocw.Backend.Schemas
{
    public class CourseBrief
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ClassName { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<(int, string)> Lecturers { get; set; }
        public string Description { get; set; }
        public string? ImageLink { get; set; }


        public static CourseBrief FromBson(BsonDocument doc)
        {
            var dict = doc.ToDictionary();
            CourseBrief ans = new()
            {
                Id = (string) dict[Definitions.KEY_CODE], 
                // TODO: name
                ClassName = (string) dict[Definitions.KEY_CLASS_NAME],
            };
            return ans;
        }
    }
}
