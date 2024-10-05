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
using System.Threading.Tasks;
using Oocw.Backend.Models;

namespace Oocw.Backend.Controllers;

[ApiController]
[Route("api/course")]
public class CourseController : ControllerBase
{
    // var defs
    [FromServices] public DatabaseService DbService { get; set; } = null!;
    [FromServices] public SearchService SearchService { get; set; } = null!;
    // api


    [HttpGet("search")]
    public async Task<ListResult<CourseBrief>> SearchCourse(
        string queryStr, 
        [FromQuery] PaginationParams pagination,
        string? lang, 
        string? restrictions, int? dispCount, int? page, string? sort, string? filter)
    {
        pagination ??= new();
        pagination.Sanitize();

        lang ??= this.TryGetLanguage();
        var list = await SearchService.SearchCourse(queryStr, pagination, lang);

        // TODO reform to course brief

        throw new NotImplementedException();
    }

    [HttpGet("list/{id}")]
    public ActionResult<IEnumerable<CourseBrief>> ByDepartment(string id, int? year, bool? byClass, string? lang, string? sort, string? filter, int? dispCount, int? page)
    {
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);
        lang = lang ?? this.TryGetLanguage();

        var crsFilter = Builders<Course>.Filter.AnyEq(x => x.Departments, id);
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
            var courses = DbService.Wrapper.Courses.Find(crsFilter);
            var clist = courses/*.Skip(dPage * dCount - dPage)*/.Limit(dCount).ToList();
            var cllist = clist.Where(x => x.Classes.Count() > 0).Select(x => x.Classes.Max().ToString());
            var classes = DbService.Wrapper.Classes.Find(Builders<Class>.Filter.In(x => x.Meta.OcwId, cllist)).ToList();

            Dictionary<string, Class> d = new(classes.Count);
            foreach (var cls in classes)
                d[cls.Meta.OcwId!] = cls;
            var ret = Enumerable.Zip(clist, cllist).Select((val) =>
            {
                var crs = val.First;
                var clsid = val.Second;
                CourseBrief b;
                if (d.ContainsKey(clsid))
                    b = CourseBrief.FromScheme(d[clsid], crs, lang).SetLecturers(d[clsid], DbService.Wrapper, lang);
                else
                    b = new(crs, lang);
                return b;
            }) ?? Enumerable.Empty<CourseBrief>();
            return Ok(ret);
        }


        throw new NotImplementedException();
    }

    [HttpGet("info/{id}")]
    public ActionResult<string> Info(string id, int? year, string? className, string? lang)
    {
        var query = _f.Eq(x => x.Code, id);
        if (className != null)
            query &= _f.Eq(x => x.ClassName, className);
        if (year != null)
            query &= _f.Eq(x => x.Year, year);


        var cls = DbService.Wrapper.Classes.Find(query).FirstOrDefault();
        var crs = cls != null ? DbService.Wrapper.GetCourseInfo(id) : null;

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

        var cls = DbService.Wrapper.Classes.Find(query).FirstOrDefault();
        var crs = cls != null ? DbService.Wrapper.GetCourseInfo(id) : null;

        if (cls == null || crs == null)
        {
            return NotFound();
        }

        return CourseBrief.FromScheme(cls, crs, lang: lang).SetLecturers(cls, lang: lang, db: DbService.Wrapper);
    }
}