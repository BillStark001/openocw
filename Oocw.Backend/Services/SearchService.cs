

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Oocw.Backend.Models;
using Oocw.Backend.Schemas;
using Oocw.Backend.Utils;
using Oocw.Database.Models;

namespace Oocw.Backend.Services;

public class SearchService
{

    [FromServices] public DatabaseService DbService { get; set; } = null!;

    public async Task<List<Course>> SearchCourse(
        string queryStr, 
        PaginationParams pagination,
        string lang)
    {

        var tokens = QueryUtils.FormSearchKeyWords(queryStr);

        var query = Builders<Course>.Filter.Text(tokens);
        var projection = Builders<Course>.Projection.MetaTextScore(x => x.Meta.SearchScore);
        var sorter = Builders<Course>.Sort.MetaTextScore("__meta__.searchScore");

        var cursor = DbService.Wrapper.Courses.Find(query)
            .Project<Course>(projection)
            .Sort(sorter)
            .Skip(pagination.Page * pagination.PageSize - pagination.Page)
            .Limit(pagination.PageSize);
        
        var list = await cursor.ToListAsync();
        return list;
    }

}