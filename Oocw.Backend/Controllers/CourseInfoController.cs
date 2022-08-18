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
    [Route("api/[controller]")]
    public class CourseInfoController : ControllerBase
    {
        // var defs

        private readonly ILogger<CourseInfoController> _logger;
        private readonly Database.Database _db;
        private readonly FilterDefinitionBuilder<BsonDocument> _f;

        // init

        public CourseInfoController(ILogger<CourseInfoController> logger)
        {
            _logger = logger;
            _db = Database.Database.Instance;
            _f = Builders<BsonDocument>.Filter;
        }

        // api

        [HttpGet("/api/course/info/{id}")]
        public ActionResult<string> Info(string id, int? year, string? className, string? lang)
        {
            var query = _f.Eq(Definitions.KEY_CODE, id);
            if (className != null)
                query &= _f.Eq(Definitions.KEY_CLASS_NAME, className);
            if (year != null)
                query &= _f.Eq(Definitions.KEY_YEAR, year);

            lang = lang ?? this.TryGetLanguage();

            var cls = _db.Classes.Find(query).FirstOrDefault();
            var crs = cls != null ? _db.GetCourseInfo(id) : null;

            if (cls == null || crs == null)
            {
                return NotFound();
            }

            return crs.ToString() + cls.ToString();

        }

        [HttpGet("/api/course/brief/{id}")]
        public ActionResult<CourseBrief> Brief(string id, int? year, string? className, string? lang)
        {
            var query = _f.Eq(Definitions.KEY_CODE, id);
            if (className != null)
                query &= _f.Eq(Definitions.KEY_CLASS_NAME, className);
            if (year != null)
                query &= _f.Eq(Definitions.KEY_YEAR, year);

            lang = lang ?? this.TryGetLanguage();

            var cls = _db.Classes.Find(query).FirstOrDefault();
            var crs = cls != null ? _db.GetCourseInfo(id) : null;

            if (cls == null || crs == null)
            {
                return NotFound();
            }

            return CourseBrief.FromBson(cls, crs, lang: lang).SetLecturers(cls, lang: lang, db: _db);
        }

        [HttpGet("/api/course/search")]
        public IEnumerable<CourseBrief> Search(string queryStr, string? restrictions, int? dispCount, int? page, string? cat = null, string? lang = null)
        {

            var tokens = QueryUtils.FormSearchKeyWords(queryStr);
            lang = lang ?? this.TryGetLanguage();
            cat = (cat ?? "").ToLower();

            int dCount = (dispCount ?? 0);
            dCount = dCount > 10 ? dCount : 10;
            dCount = dCount < 100 ? dCount : 100;

            int dPage = (page ?? 0);
            dPage = dPage > 1 ? dPage : 1;
            
            var query = Builders<BsonDocument>.Filter.Text(tokens);
            var targetDb = cat.Contains("class") ? _db.Classes : cat.Contains("course") ? _db.Courses : _db.Faculties;
            // TODO target db!
            var crs = _db.Classes.Find(query).Skip(dPage * dCount - dPage).Limit(dCount);

            IEnumerable<CourseBrief> ans = crs.ToList().Select(x =>
            {
                x.TryGetElement(Definitions.KEY_CODE, out var id);
                return CourseBrief.FromBson(x, _db.GetCourseInfo(id.Value.ToString() ?? ""), lang: lang).SetLecturers(x, lang: lang, db: _db);
            });
            return ans;
        }

        [HttpGet("/api/faculty/{id}")]
        public ActionResult<FacultyBrief> Faculty(int id, int? dispCount, int? page, string? lang = null)
        {
            lang = lang ?? this.TryGetLanguage();

            var dinfo = _db.Faculties.Find(_f.Eq(Definitions.KEY_ID, id)).FirstOrDefault();
            if (dinfo == null)
                return NotFound();

            var nameDict = dinfo[Definitions.KEY_NAME].AsBsonDocument.ToDictionary()!;

            var ans = new FacultyBrief()
            {
                Name = nameDict.TryGetTranslation(lang) ?? "",
                Names = new Dictionary<string, string>(nameDict!.Select(x => KeyValuePair.Create(x.Key, x.Value.ToString() ?? "")))
            };

            // TODO courses by id

            return ans;
        }


    }
}