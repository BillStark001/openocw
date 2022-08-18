using System.Linq;

using MeCab.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Backend.Database;
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
        private readonly Database.Database _db;
        private readonly FilterDefinitionBuilder<BsonDocument> _f;

        public CourseInfoController(ILogger<CourseInfoController> logger)
        {
            _logger = logger;
            _db = Database.Database.Instance;
            _f = Builders<BsonDocument>.Filter;
        }

        [HttpGet("/api/course/info/{id}")]
        public ActionResult<string> Info(string id, int? year, string? className)
        {
            var query = _f.Eq(Definitions.KEY_CODE, id);
            if (className != null)
                query &= _f.Eq(Definitions.KEY_CLASS_NAME, className);
            if (year != null)
                query &= _f.Eq(Definitions.KEY_YEAR, year);

            var cls = _db.Classes.Find(query).FirstOrDefault();
            var crs = cls != null ? _db.GetCourseInfo(id) : null;

            if (cls == null || crs == null)
            {
                return NotFound();
            }

            return crs.ToString() + cls.ToString();

        }

        [HttpGet("/api/course/brief/{id}")]
        public ActionResult<CourseBrief> Brief(string id, int? year, string? className)
        {
            var query = _f.Eq(Definitions.KEY_CODE, id);
            if (className != null)
                query &= _f.Eq(Definitions.KEY_CLASS_NAME, className);
            if (year != null)
                query &= _f.Eq(Definitions.KEY_YEAR, year);

            var cls = _db.Classes.Find(query).FirstOrDefault();
            var crs = cls != null ? _db.GetCourseInfo(id) : null;

            if (cls == null || crs == null)
            {
                return NotFound();
            }

            return CourseBrief.FromBson(cls, crs).SetLecturers(cls);
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
            
            var query = Builders<BsonDocument>.Filter.Text(tokens);
            var crs = _db.Classes.Find(query).Skip(dPage * dCount - dPage).Limit(dCount);

            IEnumerable<CourseBrief> ans = crs.ToList().Select(x =>
            {
                x.TryGetElement(Definitions.KEY_ID, out var id);
                return CourseBrief.FromBson(x, _db.GetCourseInfo(id.Value.ToString() ?? "")).SetLecturers(x);
            });
            return ans;
        }


    }
}