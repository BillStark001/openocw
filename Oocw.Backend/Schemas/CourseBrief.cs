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
        Id = course.CourseCode;
        Name = course.Name.Translate(lang) ?? course.Name.ForceTranslate();
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
        if (dClass.Syllabus.Version == "raw")
        {
            var rawSyl = dClass.Syllabus.GetItem()!;
            if (rawSyl.TryGetElement("desc", out var rawDesc))
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

    public CourseBrief SetLecturers(Class dClass, OocwDatabase db, string lang = "ja")
    {
        return SetLecturers(db.GetFacultyNames(dClass.Lecturers, lang));
    }
}