using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Crawler.Models;

public class CourseRecord
{
    public class Additional
    {
        public int Credit { get; set; } = -1;
        public (double, double, double, int)? Credit2 { get; set; } = null;
        public string Unit { get; set; } = "";

        public string OcwId { get; set; } = "";
    }

    public string Code { get; set; } = "XXX.X000";
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public IEnumerable<FacultyRecord> Faculties { get; set; } = [];
    public string Quarter { get; set; } = "";
    public string SyllabusUpdated { get; set; } = "";
    public string NotesUpdated { get; set; } = "";
    public int AccessRanking { get; set; } = -1;

    public CourseRecord()
    {

    }

    public CourseRecord(SyllabusRecord syl, string lang)
    {
        Code = syl.Summary.Code;
        Name = lang.ToLower() == "en" ? syl.NameEn : syl.NameJa;
        Faculties = syl.Faculties;
        Quarter = syl.Summary.Quarter;
        SyllabusUpdated = syl.Summary.SyllabusUpdated;
        NotesUpdated = syl.Summary.NotesUpdated;
    }
}
