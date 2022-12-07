using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using Oocw.Database;
using Oocw.Backend.Utils;
using Oocw.Utils;
using Oocw.Database.Models;

namespace Oocw.Backend.Schemas;

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

    public CourseBrief() { }

    public CourseBrief(Course course, string lang = "ja")
    {
        Id = course.Code;
        Name = course.Name.Translate(lang) ?? course.Name.ForceTranslate();
    }


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

    public static CourseBrief FromScheme(Class dClass, Course? dCourse = null, string lang = "ja")
    {
        CourseBrief ans = new()
        {
            Id = dClass.Code, 
            ClassName = dClass.ClassName
        };
        if (dCourse != null)
        {
            ans.Name = dCourse.Name.Translate(lang) ?? dCourse.Name.ForceTranslate();
        }

        // description
        if (dClass.Syllabus.Version == Definitions.VAL_VER_RAW)
        {
            var rawSyl = dClass.Syllabus.GetItem()!;
            if (rawSyl.TryGetElement(Definitions.KEY_SYL_DESC, out var rawDesc))
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

    public CourseBrief SetLecturers(BsonDocument dClass, DBWrapper db, string lang = "ja")
    {
        dClass.TryGetElement(Definitions.KEY_LECTURERS, out var lecturers);
        return SetLecturers(db.GetFacultyNames(lecturers.Value.AsBsonArray.Values.Select(x => x.BsonType == BsonType.Int32 || x.BsonType == BsonType.Int64 ? x.ToInt32() : -1), lang));
    }

    public CourseBrief SetLecturers(Class dClass, DBWrapper db, string lang = "ja")
    {
        return SetLecturers(db.GetFacultyNames(dClass.Lecturers, lang));
    }
}