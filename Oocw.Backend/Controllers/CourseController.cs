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
using Oocw.Backend.Api;
using System.Net;

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
        [FromQuery] CourseFilter filter,
        [FromQuery] PaginationParams pagination,
        string? lang)
    {
        filter ??= new();
        pagination ??= new();
        pagination.Sanitize();
        lang ??= this.TryGetLanguage();

        var list = await SearchService.SearchCourse(filter, pagination, lang);

        // TODO reform to course brief

        throw new NotImplementedException();
    }

    [HttpGet("list/{id}")]
    public Task<ListResult<CourseBrief>> ListCourseByDepartment(
        string? id,
        [FromQuery] CourseFilter filter,
        [FromQuery] PaginationParams pagination,
        string? lang)
    {
        filter ??= new();
        filter.DepartmentExact = [id ?? ""];
        return ListCourse(filter, pagination, lang);
    }

    [HttpGet("list")]
    public async Task<ListResult<CourseBrief>> ListCourse(
        [FromQuery] CourseFilter filter,
        [FromQuery] PaginationParams pagination,
        string? lang)
    {
        filter ??= new();
        pagination ??= new();
        pagination.Sanitize();
        lang ??= this.TryGetLanguage();

        var cursor = DbService.Wrapper.Courses.Find(filter.GetCourseFilterDefinition());
        var pagedCursor = cursor
            .Skip(pagination.Page * pagination.PageSize - pagination.Page)
            .Limit(pagination.PageSize);
        
        var list = await pagedCursor.ToListAsync();

        throw new NotImplementedException();
    }

    [HttpGet("info/{code}")]
    public async Task<CourseSchema> GetCourseInfo(string code, string? classId, string? lang)
    {
        lang ??= this.TryGetLanguage();

        var crs = await DbService.Wrapper.Courses.Find(x => x.CourseCode == code).FirstOrDefaultAsync();
        var cls = !string.IsNullOrWhiteSpace(classId) && crs != null
            ? await DbService.Wrapper.Classes.Find(x => x.CourseId == crs.Id && x.Id == classId).FirstOrDefaultAsync()
            : null;
        
        if (crs == null || (!string.IsNullOrWhiteSpace(classId) && cls == null))
        {
            throw new ApiException((int) HttpStatusCode.NotFound);
        }

        // TODO implement a better logic
        throw new NotImplementedException();
        // return crs.ToString() + cls?.ToString() ?? "";

    }


    [HttpPost("edit")]
    public async Task Edit(string? lang, [FromBody] CourseSchema course) {
        
        lang ??= this.TryGetLanguage();
        if (string.IsNullOrWhiteSpace(course.Id)) {
            throw new ApiException((int) HttpStatusCode.NotFound);
        }

        throw new NotImplementedException();
    }
    
}