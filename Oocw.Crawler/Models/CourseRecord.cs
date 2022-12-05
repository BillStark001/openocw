using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Crawler.Models;

public class CourseRecord
{
    public string Code { get; set; } = "XXX.X000";
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public IEnumerable<FacultyRecord> Faculties { get; set; } = Enumerable.Empty<FacultyRecord>();
    public string Quarter { get; set; } = "";
    public string SyllabusUpdated { get; set; } = "";
    public string NotesUpdated { get; set; } = "";
    public int AccessRanking { get; set; } = -1;
}
