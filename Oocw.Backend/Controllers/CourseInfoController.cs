using System.Linq;

using MeCab.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Backend.Schemas;
using Oocw.Backend.Utils;

namespace Oocw.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseInfoController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<CourseInfoController> _logger;

        public CourseInfoController(ILogger<CourseInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/api/course/info/{id}")]
        public string Info(string id, int? year, string? className)
        {
            return $"## Test Markdown\n### {id} / {year} / {className}";
        }

        [HttpGet("/api/course/brief/{id}")]
        public CourseBrief Brief(string id, int? year, string? className)
        {
            return new()
            {
                Id = id,
                Name = "",
                ClassName = className
            };
        }

        [HttpGet("/api/course/search")]
        public IEnumerable<CourseBrief> Search(string queryStr, string? restrictions, int? dispCount, int? page, bool byClass = false)
        {
            var tokens = QueryUtils.FormSearchKeyWords(queryStr);
            

            int dCount = (dispCount ?? 0);
            dCount = dCount > 10 ? dCount : 10;
            dCount = dCount < 100 ? dCount : 100;

            int dPage = (page ?? 0);
            dPage = dPage > 1 ? dPage : 1;
            
            var db = Database.Database.Instance.Wrapper;
            var query = Builders<BsonDocument>.Filter.Text(tokens);
            var crs = db.Classes.Find(query).Skip(dPage * dCount - dPage).Limit(dCount);

            IEnumerable<CourseBrief> ans = crs.ToList().Select(x => CourseBrief.FromBson(x));
            return ans;
        }


    }
}