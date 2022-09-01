using Oocw.Database;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oocw.Utils;

public static class UserUtils
{
    public const string REGEX_EMAIL = @"^([A-Za-z0-9._%+-]+)@([A-Za-z0-9.-]+\.[A-Z|a-z]{2,})$";
    public const string REGEX_UNAME = @"^[\x20-\x7E]{6,127}$";
    public const string REGEX_PWD = @"^[\x20-\x7E]{6,32}$";
    public static readonly Regex RegexEmail;
    public static readonly Regex RegexUserName;
    public static readonly Regex RegexPassword;
    public static readonly IEnumerable<Regex> RegexPasswords;
    public static readonly SHA256 sha256Instance;

    static UserUtils()
    {
        RegexEmail = new Regex(REGEX_EMAIL, RegexOptions.Compiled);
        RegexUserName = new Regex(REGEX_UNAME, RegexOptions.Compiled);
        RegexPassword = new Regex(REGEX_PWD, RegexOptions.Compiled);
        RegexPasswords = new List<Regex>()
        {
            new Regex(@"[0-9]", RegexOptions.Compiled),
            new Regex(@"[a-z]", RegexOptions.Compiled),
            new Regex(@"[A-Z]", RegexOptions.Compiled),
            new Regex(@"[\x20-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]", RegexOptions.Compiled),
        }.AsReadOnly();
        sha256Instance = SHA256.Create();
    }

    // validity check

    public static bool IsValidEmail(string? inStr)
    {
        return RegexEmail.IsMatch(inStr ?? "");
    }

    public static bool IsValidUsername(string? inStr)
    {
        return RegexUserName.IsMatch(inStr ?? "");
    }

    public static bool IsValidPassword(string? inStr)
    {
        return RegexPassword.IsMatch(inStr ?? "");
    }

    public static bool IsValidPasswordStrict(string? inStr)
    {
        if (!IsValidPassword(inStr))
            return false;
        int w = 0;
        foreach (var re in RegexPasswords)
            if (re.IsMatch(inStr!))
                ++w;
        return w > 2;
    }

    // password enc/vrf etc.
    public static string encryptPassword(string? inStr)
    {
        inStr = inStr ?? "";
        var bytes = sha256Instance.ComputeHash(Encoding.UTF8.GetBytes(inStr));
        StringBuilder sb = new();
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }


    public static bool verifyPassword(string? inPwd, string? realPwdEnc)
    {
        return realPwdEnc != null && encryptPassword(inPwd) == realPwdEnc;
    }

    public static User Register(this DBWrapper db, string uname, string pwdPlain)
    {
        return db.PutUser(uname, encryptPassword(pwdPlain));
    }

}
