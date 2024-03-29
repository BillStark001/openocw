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
[Route("api/search")]
public class SearchController : ControllerBase
{
    // var defs

    private readonly ILogger<QueryController> _logger;
    private readonly DatabaseService _db;
    private readonly FilterDefinitionBuilder<BsonDocument> _f;

    // init

    public SearchController(ILogger<QueryController> logger, DatabaseService db)
    {
        _logger = logger;
        _db = db;
        _f = Builders<BsonDocument>.Filter;
    }

    // api


    [HttpGet("class")]
    public IEnumerable<CourseBrief> SearchClass(string queryStr, string? restrictions, int? dispCount, int? page, string? lang, string? sort, string? filter)
    {

        var tokens = QueryUtils.FormSearchKeyWords(queryStr);
        lang = lang ?? this.TryGetLanguage();
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);

        var query = Builders<Class>.Filter.Text(tokens);
        var projection = Builders<Class>.Projection.MetaTextScore(Definitions.MetaTextScoreTarget);
        var sorter = Builders<Class>.Sort.MetaTextScore(Definitions.MetaTextScoreTarget);

        // TODO target db!
        var cls = _db.Wrapper.Classes.Find(query).Project<Class>(projection).Sort(sorter);
        cls = cls.Skip(dPage * dCount - dPage).Limit(dCount);

        IEnumerable<CourseBrief> ans = cls.ToList().Select(x =>
        {
            return CourseBrief.FromScheme(x, _db.Wrapper.GetCourseInfo(x.Code), lang: lang).SetLecturers(x, lang: lang, db: _db.Wrapper);
        });
        return ans;
    }

    [HttpGet("course")]
    public IEnumerable<CourseBrief> SearchCourse(string queryStr, string? restrictions, int? dispCount, int? page, string? lang, string? sort, string? filter)
    {

        var tokens = QueryUtils.FormSearchKeyWords(queryStr);
        lang = lang ?? this.TryGetLanguage();
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);

        var query = Builders<Course>.Filter.Text(tokens);
        var projection = Builders<Course>.Projection.MetaTextScore(Definitions.MetaTextScoreTarget);
        var sorter = Builders<Course>.Sort.MetaTextScore(Definitions.MetaTextScoreTarget);

        // TODO target db!
        var cls = _db.Wrapper.Courses.Find(query).Project<Course>(projection).Sort(sorter);
        cls = cls.Skip(dPage * dCount - dPage).Limit(dCount);

        throw new NotImplementedException();
    }

    [HttpGet("faculty")]
    public IEnumerable<FacultyBrief> SearchFaculty(string queryStr, string? restrictions, int? dispCount, int? page, string? lang, string? sort, string? filter)
    {
        var tokens = QueryUtils.FormSearchKeyWords(queryStr);
        lang = lang ?? this.TryGetLanguage();
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);

        var query = Builders<Faculty>.Filter.Text(tokens);
        var fct = _db.Wrapper.Faculties.Find(query).Skip(dPage * dCount - dPage).Limit(dCount);

        IEnumerable<FacultyBrief> ans = fct.ToList().Select(x => new FacultyBrief(x, lang));
        return ans;

    }



}