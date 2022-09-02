using Oocw.Backend.Schemas;
using Oocw.Database.Models;
using System.Collections.Generic;
using System.Linq;

namespace Oocw.Backend.Schemas;
public class FacultyBrief
{
    public string Name { get; set; } = "";
    public IDictionary<string, string> Names { get; set; } = new Dictionary<string, string>();
    public IEnumerable<CourseBrief> Courses { get; set; } = Enumerable.Empty<CourseBrief>();

    public FacultyBrief() { }

    public FacultyBrief(Faculty f, string? lang = null)
    {
        Name = f.Name.Translate(lang ?? "ja") ?? f.Name.ForceTranslate();
        Names = f.Name.AsDictionary(false);
    }
}