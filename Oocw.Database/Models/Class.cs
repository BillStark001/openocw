using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Oocw.Database.Models.Technical;
using System.Xml.Linq;
using System.Linq.Expressions;
using Oocw.Base;

namespace Oocw.Database.Models;

public class Class : DataModel
{


    public string CourseId {get; set;} = "";
    public string ClassName { get; set; } = "";
    public HashSet<string> Lecturers {get; set;} = [];
    public string Language { get; set; } = "null";
    public MultiLingualField Content { get; set; } = new();


}