

using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Oocw.Backend.Models;
using Oocw.Backend.Schemas;
using Oocw.Backend.Utils;
using Oocw.Database.Models;
using System;

namespace Oocw.Backend.Services;


public class CourseFilter {

    // exact match
    public IList<string>? DepartmentExact { get; set; }
    public IList<string>? LecturerExact { get; set; }
    public IList<string>? TagExact { get; set; }
    public string? CodeExact {get; set;}

    // vague match
    public string? CodeVague {get; set;}
    public string? InfoVague {get; set;}
    public string? ContentVague {get; set;}


    public bool NeedsTextScore => !string.IsNullOrWhiteSpace(InfoVague) || !string.IsNullOrWhiteSpace(ContentVague);

    public FilterDefinition<Course>? GetCourseFilterDefinition() {

        List<FilterDefinition<Course>> records = [];
        var f = Builders<Course>.Filter;

        if ((DepartmentExact?.Count ?? 0) > 0) {
            records.Add(f.AnyIn(x => x.Departments, DepartmentExact));
        }
        if ((LecturerExact?.Count ?? 0) > 0) {
            records.Add(f.AnyIn(x => x.Lecturers, LecturerExact));
        }
        if ((TagExact?.Count ?? 0) > 0) {
            records.Add(f.AnyIn(x => x.Tags, TagExact));
        }
        if (!string.IsNullOrWhiteSpace(CodeExact)) {
            records.Add(f.Eq(x => x.CourseCode, CodeExact));
        }
        if (!string.IsNullOrWhiteSpace(CodeVague)) {
            records.Add(f.StringIn(x => x.CourseCode, CodeVague));
        }
        
        if (records.Count == 0) {
            return null;
        }
        return f.And(records);
    }

    public FilterDefinition<CourseRecord>? GetRecordFilterDefinition(string lang = "ja") {

        List<FilterDefinition<CourseRecord>> records = [];

        // TODO other parts

        var f = Builders<CourseRecord>.Filter;
        if (!string.IsNullOrWhiteSpace(CodeExact)) {
            records.Add(f.Eq(x => x.CodeRecord, CodeExact));
        }
        if (!string.IsNullOrWhiteSpace(CodeVague)) {
            records.Add(f.StringIn(x => x.CodeRecord, CodeVague));
        }
        if (!string.IsNullOrWhiteSpace(InfoVague)) {
            records.Add(f.StringIn(x => x.InfoRecord, InfoVague));
        }
        if (!string.IsNullOrWhiteSpace(ContentVague)) {
            records.Add(f.Text(QueryUtils.FormSearchKeyWords(ContentVague, lang)));
        }

        if (records.Count == 0) {
            return null;
        }
        return f.And(records);
    }
    
}

public class SearchService
{

    [FromServices] public DatabaseService DbService { get; set; } = null!;

    public async Task<List<CourseRecord>> SearchCourse(
        CourseFilter filter,
        PaginationParams pagination,
        string lang,
        CancellationToken cancellationToken = default
    )
    {
        filter ??= new();
        lang ??= "en";

        var query = filter.GetRecordFilterDefinition(lang);

        var projection = Builders<CourseRecord>.Projection.MetaTextScore(x => x.SearchScore);
        var sorter = Builders<CourseRecord>.Sort.MetaTextScore("searchScore");

        var cursor = DbService.Wrapper.CourseRecords.Find(query);

        if (filter.NeedsTextScore) {
            cursor = cursor
                .Project<CourseRecord>(projection)
                .Sort(sorter);
        }

        var pagedCursor = cursor
            .Skip(pagination.Page * pagination.PageSize - pagination.Page)
            .Limit(pagination.PageSize);

        var list = await pagedCursor.ToListAsync(cancellationToken: cancellationToken);
        return list;
    }

    public static readonly IList<string> Languages = ["en", "ja-JP", "zh-CN"];

    public async Task MarkCourseRecordDirty(string courseId, CancellationToken cancellationToken = default)
    {
        foreach (var lang in Languages)
        {
            var res = await DbService.Wrapper.CourseRecords.UpdateManyAsync(
                x => x.CourseId == courseId, Builders<CourseRecord>.Update.Set(x => x.Dirty, true)
                , cancellationToken: cancellationToken
                );

            if (res.ModifiedCount == 0)
            {
                await DbService.Wrapper.CourseRecords.InsertOneAsync(new CourseRecord
                {
                    CourseId = courseId,
                    Language = lang,
                    Dirty = true,
                }, cancellationToken: cancellationToken);
            }
        }
    }

}