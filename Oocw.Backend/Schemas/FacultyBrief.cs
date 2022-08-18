namespace Oocw.Backend.Schemas
{
    public class FacultyBrief
    {
        public string Name { get; set; } = "";
        public IDictionary<string, string> Names { get; set; } = new Dictionary<string, string>();
        public IEnumerable<CourseBrief> Courses { get; set; } = Enumerable.Empty<CourseBrief>();
    }
}