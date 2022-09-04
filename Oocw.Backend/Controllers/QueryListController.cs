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
using System;

namespace Oocw.Backend.Controllers;

[ApiController]
[Route("api/list")]
public class QueryListController : ControllerBase
{
    // var defs

    private readonly ILogger<QueryController> _logger;
    private readonly DatabaseService _db;
    private readonly FilterDefinitionBuilder<Class> _f;

    // init

    public QueryListController(ILogger<QueryController> logger, DatabaseService db)
    {
        _logger = logger;
        _db = db;
        _f = Builders<Class>.Filter;
    }

    // api

    [HttpGet("dept/{id}")]
    public ActionResult<IEnumerable<CourseBrief>> ByDepartment(string id, int? year, bool? byClass, string? lang, string? sort, string? filter, int? dispCount, int? page)
    {
        var ids = id.Split(',');
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);
        lang = lang ?? this.TryGetLanguage();

        var crsFilter = Builders<Course>.Filter.In(x => x.Unit.Key, ids);
        if (year != null)
            crsFilter = Builders<Course>.Filter.And(
                crsFilter,
                Builders<Course>.Filter.ElemMatch(x => x.Classes, c => c / 100000 == year)
                );

        if (byClass != null && byClass.Value)
        {
            throw new NotImplementedException();
        }
        else
        {
            var courses = _db.Wrapper.Courses.Find(crsFilter);
            var clist = courses.Skip(dPage * dCount - dPage).Limit(dCount).ToList();
            var cllist = clist.Select(x => x.Classes.Count() > 0 ? x.Classes.Max() : -1);
            var classes = _db.Wrapper.Classes.Find(Builders<Class>.Filter.In(x => x.Meta.OcwId, cllist)).ToList();
            Dictionary<int, Class> d = new(classes.Count);
            foreach (var cls in classes)
                d[cls.Meta.OcwId] = cls;
            var ret = Enumerable.Zip(clist, cllist).Select((val) =>
            {
                var crs = val.First;
                var clsid = val.Second;
                CourseBrief b;
                if (d.ContainsKey(clsid))
                    b = CourseBrief.FromScheme(d[clsid], crs, lang).SetLecturers(d[clsid], _db.Wrapper, lang);
                else
                    b = new(crs, lang);
                return b;
            }) ?? Enumerable.Empty<CourseBrief>();
            return Ok(ret);
        }


        throw new NotImplementedException();
    }

    [HttpGet("faculty/{id}")]
    public ActionResult<IEnumerable<CourseBrief>> ByFaculty(int id, int? year, bool? byClass, string? lang, string? sort, string? filter, int? dispCount, int? page)
    {
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);
        lang = lang ?? this.TryGetLanguage();

        throw new NotImplementedException();
    }

}