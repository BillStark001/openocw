using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class Course : DataModel
{

    public MultiLingualField Name { get; set; } = new();

    public MultiLingualField Content { get; set; } = new();
    public string? Image { get; set; }

    public string CourseCode { get; set; } = "";

    public List<string> Departments { get; set; } = [];
    public List<string> Lecturers { get; set; } = [];
    public List<string> Tags { get; set; } = [];

    public int Credit { get; set; }

}
