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
    public ActionResult<IEnumerable<CourseBrief>> ByDepartment(string id, int? year, bool? byClass, string? lang, string? sort, string? filter)
    {
        throw new NotImplementedException();
    }

    [HttpGet("faculty/{id}")]
    public ActionResult<IEnumerable<CourseBrief>> ByFaculty(int id, int? year, bool? byClass, string? lang, string? sort, string? filter)
    {
        throw new NotImplementedException();
    }

}