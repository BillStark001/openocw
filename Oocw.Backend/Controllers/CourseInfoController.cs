using System.Linq;
using System.Collections.Generic;

using MeCab.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Database;
using Oocw.Backend.Schemas;
using Oocw.Backend.Utils;
using Oocw.Backend.Services;
using Oocw.Utils;

namespace Oocw.Backend.Controllers;

[ApiController]
[Route("api/course")]
public class CourseInfoController : ControllerBase
{
    // var defs

    private readonly ILogger<CourseInfoController> _logger;
    private readonly DatabaseService _db;
    private readonly FilterDefinitionBuilder<BsonDocument> _f;

    // init

    public CourseInfoController(ILogger<CourseInfoController> logger, DatabaseService db)
    {
        _logger = logger;
        _db = db;
        _f = Builders<BsonDocument>.Filter;
    }

    // api

    [HttpGet("info/{id}")]
    public ActionResult<string> Info(string id, int? year, string? className, string? lang)
    {
        var query = _f.Eq(Definitions.KEY_CODE, id);
        if (className != null)
            query &= _f.Eq(Definitions.KEY_CLASS_NAME, className);
        if (year != null)
            query &= _f.Eq(Definitions.KEY_YEAR, year);

        lang = lang ?? this.TryGetLanguage();

        var cls = _db.Wrapper.Classes.Find(query).FirstOrDefault();
        var crs = cls != null ? _db.Wrapper.GetCourseInfo(id) : null;

        if (cls == null || crs == null)
        {
            return NotFound();
        }

        return crs.ToString() + cls.ToString();

    }

    [HttpGet("brief/{id}")]
    public ActionResult<CourseBrief> Brief(string id, int? year, string? className, string? lang)
    {
        var query = _f.Eq(Definitions.KEY_CODE, id);
        if (className != null)
            query &= _f.Eq(Definitions.KEY_CLASS_NAME, className);
        if (year != null)
            query &= _f.Eq(Definitions.KEY_YEAR, year);

        lang = lang ?? this.TryGetLanguage();

        var cls = _db.Wrapper.Classes.Find(query).FirstOrDefault();
        var crs = cls != null ? _db.Wrapper.GetCourseInfo(id) : null;

        if (cls == null || crs == null)
        {
            return NotFound();
        }

        return CourseBrief.FromBson(cls, crs, lang: lang).SetLecturers(cls, lang: lang, db: _db.Wrapper);
    }

    [HttpGet("search")]
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
        var targetDb = cat.Contains("class") ? _db.Wrapper.Classes : cat.Contains("course") ? _db.Wrapper.Courses : _db.Wrapper.Faculties;
        // TODO target db!
        var crs = _db.Wrapper.Classes.Find(query).Skip(dPage * dCount - dPage).Limit(dCount);

        IEnumerable<CourseBrief> ans = crs.ToList().Select(x =>
        {
            x.TryGetElement(Definitions.KEY_CODE, out var id);
            return CourseBrief.FromBson(x, _db.Wrapper.GetCourseInfo(id.Value.ToString() ?? ""), lang: lang).SetLecturers(x, lang: lang, db: _db.Wrapper);
        });
        return ans;
    }

    [HttpGet("/api/faculty/{id}")]
    public ActionResult<FacultyBrief> Faculty(int id, int? dispCount, int? page, string? lang = null)
    {
        lang = lang ?? this.TryGetLanguage();

        var dinfo = _db.Wrapper.Faculties.Find(_f.Eq(Definitions.KEY_ID, id)).FirstOrDefault();
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