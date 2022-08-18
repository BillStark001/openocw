﻿using System.Linq;
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
        public string Description { get; set; } = "No Description.";
        public string? ImageLink { get; set; }



        public static CourseBrief FromBson(BsonDocument dClass, BsonDocument? dCourse = null, string lang = "ja")
        {
            CourseBrief ans = new()
            {
                Id = dClass[Definitions.KEY_CODE].AsString,
                ClassName = dClass[Definitions.KEY_CLASS_NAME].AsString,
            };
            if (dCourse != null)
            {
                ans.Name = dCourse[Definitions.KEY_NAME].TryGetTranslation(lang) ?? "";
            }

            // description
            var sylVer = dClass[Definitions.KEY_SYLLABUS][Definitions.KEY_VERSION].AsString;
            if (sylVer == Definitions.VAL_VER_RAW)
            {
                var rawSyl = dClass[Definitions.KEY_SYLLABUS][sylVer];
                if (rawSyl.AsBsonDocument.TryGetElement(Definitions.KEY_SYL_DESC, out var rawDesc))
                {
                    var desc = rawDesc.Value.TryGetTranslation(lang);
                    if (!string.IsNullOrWhiteSpace(desc))
                        ans.Description = desc;
                }
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
