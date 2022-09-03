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
using Oocw.Database.Models;

namespace Oocw.Backend.Controllers;

[ApiController]
[Route("api/info")]
public class QueryController : ControllerBase
{
    // var defs

    private readonly ILogger<QueryController> _logger;
    private readonly DatabaseService _db;
    private readonly FilterDefinitionBuilder<Class> _f;

    // init

    public QueryController(ILogger<QueryController> logger, DatabaseService db)
    {
        _logger = logger;
        _db = db;
        _f = Builders<Class>.Filter;
    }

    // api

    [HttpGet("course/{id}")]
    public ActionResult<string> Info(string id, int? year, string? className, string? lang)
    {
        var query = _f.Eq(x => x.Code, id);
        if (className != null)
            query &= _f.Eq(x => x.ClassName, className);
        if (year != null)
            query &= _f.Eq(x => x.Year, year);

        lang = lang ?? this.TryGetLanguage();

        var cls = _db.Wrapper.Classes.Find(query).FirstOrDefault();
        var crs = cls != null ? _db.Wrapper.GetCourseInfo(id) : null;

        if (cls == null || crs == null)
        {
            return NotFound();
        }

        return crs.ToString() + cls.ToString();

    }

    [HttpGet("coursebrief/{id}")]
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

        return CourseBrief.FromBson2(cls, crs, lang: lang).SetLecturers(cls, lang: lang, db: _db.Wrapper);
    }

    [HttpGet("faculty/{id}")]
    public ActionResult<FacultyBrief> Faculty(int id, string? lang = null)
    {
        lang = lang ?? this.TryGetLanguage();

        var dinfo = _db.Wrapper.Faculties.Find(x => x.Id == id).FirstOrDefault();
        if (dinfo == null)
            return NotFound();

        return new FacultyBrief(dinfo, lang);
    }

}