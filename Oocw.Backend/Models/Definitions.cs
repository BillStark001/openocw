namespace Oocw.Backend.Models;
public static class Definitions
{
    // codes
    public static readonly (int, string) CODE_SUCC = (0, "Done.");
    public static readonly (int, string) CODE_ERR_BAD_ARGS = (1, "Missing Arguments: {0}.");
    public static readonly (int, string) CODE_ERR_BAD_PARSE = (2, "Failed to parse arguments: {0}.");
        
    public static readonly (int, string) CODE_ERR_UNAME_CONFLICT = (3, "Username conflict: {0}.");
    public static readonly (int, string) CODE_ERR_INVALID_UNAME = (4, "Invalid user name: {0}. The user name should have and only have 6~127 printable ASCII characters.");
    public static readonly (int, string) CODE_ERR_INVALID_PWD = (5, "Invalid Password. The password should have and only have 6~32 printable ASCII characters.");
    public static readonly (int, string) CODE_ERR_INVALID_PWD_STRICT = (5, "Invalid Password. The password should have and only have 6~32 characters including at least 3 of the following: A-Z, a-z, 0-9 and other printable ASCII characters.");

    public static readonly (int, string) CODE_ERR_BAD_UNAME_OR_PWD = (16, "Invalid user name or password.");
    public static readonly (int, string) CODE_ERR_AUTH_FAILED = (17, "Authentication Failed");


    public static readonly (int, string) CODE_ERR_DB_ERR = (114514, "Database Internal Error. Please report this to the developer. / {0}");

    // keys

    public const string KEY_REFRESH_TOKEN = ".Oocw.Token.Refresh";
    public const string KEY_ACCESS_TOKEN = ".Oocw.Token.Access";
    public const string KEY_ACCESS_TOKEN_FORMAT = ".Oocw.Token.Access.{0}";
}