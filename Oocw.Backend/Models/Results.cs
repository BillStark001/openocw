namespace Oocw.Backend.Models;

public class StandardResult
{
    public int Code { get; private set; } = 0;
    public string Info { get; private set; } = "Done.";

    public StandardResult(int code, string info)
    {
        Code = code;
        Info = info;
    }

    public StandardResult((int, string) code, params object?[] forms): this(code.Item1, string.Format(code.Item2, forms)) { }
}

public class AuthResult: StandardResult
{
    public string Token { get; private set; } = null!;
    
    public AuthResult(string token): base(Definitions.CODE_SUCC)
    {
        Token = token;
    }
}