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
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public List<string> Tags { get; set; } = [];

    public List<EntityReference> Classes { get; set; } = [];
    public List<EntityReference> Lecturers { get; set; } = [];

    public string Description { get; set; } = "No Description.";
    public string? Image { get; set; }

}