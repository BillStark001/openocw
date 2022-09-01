using Oocw.Backend.Schemas;
using System.Collections.Generic;
using System.Linq;

namespace Oocw.Backend.Schemas;
public class FacultyBrief
{
    public string Name { get; set; } = "";
    public IDictionary<string, string> Names { get; set; } = new Dictionary<string, string>();
    public IEnumerable<CourseBrief> Courses { get; set; } = Enumerable.Empty<CourseBrief>();
}