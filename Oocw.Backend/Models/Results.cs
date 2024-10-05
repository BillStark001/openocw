using System.Collections.Generic;
using System.Linq;

namespace Oocw.Backend.Models;

public class StandardResult
{
    public int Code { get; set; } = 0;
    public string Info { get; set; } = DEFAULT_INFO;

    public const string DEFAULT_INFO = "Done.";

    public StandardResult(int code, string info)
    {
        Code = code;
        Info = info;
    }

    public StandardResult((int, string) code, params object?[] forms): this(code.Item1, string.Format(code.Item2, forms)) { }
}

public class ListResult<T>(IEnumerable<T>? list) : StandardResult(0, DEFAULT_INFO)
{
    public List<T> List { get; set; } = list?.ToList() ?? [];
}

public class AuthResult: StandardResult
{
    public string Token { get; private set; } = null!;
    
    public AuthResult(string token): base(Definitions.CODE_SUCC)
    {
        Token = token;
    }
}