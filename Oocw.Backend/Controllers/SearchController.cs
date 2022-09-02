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
    public IEnumerable<CourseBrief> Search(string queryStr, string? restrictions, int? dispCount, int? page, string? lang = null)
    {

        var tokens = QueryUtils.FormSearchKeyWords(queryStr);
        lang = lang ?? this.TryGetLanguage();
        var (dCount, dPage) = QueryUtils.GetPageInfo(dispCount, page);

        var query = Builders<Class>.Filter.Text(tokens);
        var projection = Builders<Class>.Projection.MetaTextScore(Definitions.MetaTextScoreTarget);
        var sort = Builders<Class>.Sort.MetaTextScore(Definitions.MetaTextScoreTarget);

        // TODO target db!
        var cls = _db.Wrapper.Classes.Find(query).Project<Class>(projection).Sort(sort);
        cls = cls.Skip(dPage * dCount - dPage).Limit(dCount);

        IEnumerable<CourseBrief> ans = cls.ToList().Select(x =>
        {
            return CourseBrief.FromBson2(x, _db.Wrapper.GetCourseInfo(x.Code), lang: lang).SetLecturers(x, lang: lang, db: _db.Wrapper);
        });
        return ans;
    }


    [HttpGet("faculty")]
    public IEnumerable<FacultyBrief> SearchFaculty(string queryStr, string? restrictions, int? dispCount, int? page, string? lang = null)
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