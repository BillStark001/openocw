using System.Linq;
using MongoDB.Bson;
using Oocw.Backend.Database;
using Oocw.Backend.Utils;

namespace Oocw.Backend.Schemas
{
    public class CourseBrief
    {
        public class Lecturer
        {
            public int Id { get; set; } = -1;
            public string Name { get; set; } = "";
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? ClassName { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<Lecturer> Lecturers { get; set; } = Enumerable.Empty<Lecturer>();
        public string Description { get; set; } = "";
        public string? ImageLink { get; set; }



        public static CourseBrief FromBson(BsonDocument dClass, BsonDocument? dCourse)
        {
            var dcrs = dCourse != null ? dCourse.ToDictionary() : null;
            var dcls = dClass.ToDictionary();
            CourseBrief ans = new()
            {
                Id = (string)dcls[Definitions.KEY_CODE],
                ClassName = (string)dcls[Definitions.KEY_CLASS_NAME],
            };
            if (dcrs != null)
            {
                ans.Name = dcrs[Definitions.KEY_NAME].TryGetTranslation() ?? "";
            }
            return ans;
        }

        public CourseBrief SetLecturers(IEnumerable<(int, string)> lects)
        {
            Lecturers = lects.Select(x => new Lecturer() { Id = x.Item1, Name = x.Item2 });
            return this;
        }

        public CourseBrief SetLecturers(BsonDocument dClass, string lang = "ja", Database.Database? db = null)
        {
            dClass.TryGetElement(Definitions.KEY_LECTURERS, out var lecturers);
            db = db ?? Database.Database.Instance;
            return SetLecturers(db.GetFacultyNames(lecturers.Value.AsBsonArray.Values.Select(x => x.BsonType == BsonType.Int32 || x.BsonType == BsonType.Int64 ? x.ToInt32() : -1), lang));
        }
    }
}
