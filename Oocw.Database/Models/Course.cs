using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Oocw.Base;
using Oocw.Database.Models.Technical;

namespace Oocw.Database.Models;

public class Course : DataModel
{
    [BsonElement(CourseMetadata.KEY_META)]
    public CourseMetadata Meta { get; set; } = new();

    // technical


    public double AccessRank { get; set; }

    // info

    public MultiLingualField Name { get; set; } = new();

    public MultiLingualField Content { get; set; } = new();


    public string CourseCode { get; set; } = "";

    public HashSet<string> Departments { get; set; } = [];
    public HashSet<string> Lecturers { get; set; } = [];
    public HashSet<string> Tags { get; set; } = [];

    public (double, double, double, int) Credit { get; set; }

    [Obsolete]
    public List<int> Classes { get; set; } = [];

}
