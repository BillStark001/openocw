

using System.Collections.Generic;

namespace Oocw.Backend.Schemas;


public class CourseSchema
{

    public string Id { get; set; } = "";
    public string Name { get; set; } = "";

    public string CourseCode { get; set; } = "";
    public int Credit { get; set; }

    public List<string> Departments { get; set; } = [];
    public List<string> Lecturers { get; set; } = [];
    public List<string> Tags { get; set; } = [];

    public string Content { get; set; } = "";
    public string? ImageLink { get; set; }

    public List<EntityReference> Classes { get; set; } = [];

}
