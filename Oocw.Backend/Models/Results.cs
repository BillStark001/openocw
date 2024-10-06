using System.Collections.Generic;
using System.Linq;

namespace Oocw.Backend.Models;


public class ListResult<T>(IEnumerable<T>? list)
{
    public List<T> List { get; set; } = list?.ToList() ?? [];
    public int? TotalPage { get; set; }
    public int? TotalCount { get; set; }
}

public class ItemResult<T>(T? item) 
{
    public T? Item { get; set; } = item;
}

public class AuthResult(string token)
{
    public string Token { get; private set; } = token;
}