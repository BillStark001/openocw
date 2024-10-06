
using System.Collections.Generic;

namespace Oocw.Backend.Schemas;


public class ClassSchema
{

    public string Id { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string ClassName { get; set; } = "";

    public List<string> Lecturers { get; set; } = [];

    public string Language { get; set; } = "null";

    public string Content { get; set; } = "";
}