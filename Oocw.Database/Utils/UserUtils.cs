using Oocw.Database;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oocw.Database.Utils;

public static class UserUtils
{
    public static readonly Regex RegexEmail;
    public static readonly Regex RegexUserName;
    public static readonly Regex RegexPassword;
    public static readonly IEnumerable<Regex> RegexPasswords;
    public const int SALT_SIZE = 16;
    public const int HASH_SIZE = 32;
    public const int PBKDF2_ITERATIONS = 10000;

    static UserUtils()
    {
        RegexEmail = new Regex(@"^([A-Za-z0-9._%+-]+)@([A-Za-z0-9.-]+\.[A-Z|a-z]{2,})$", RegexOptions.Compiled);
        RegexUserName = new Regex(@"^[\x20-\x7E]{6,127}$", RegexOptions.Compiled);
        RegexPassword = new Regex(@"^[\x20-\x7E]{6,32}$", RegexOptions.Compiled);
        RegexPasswords = new List<Regex>()
        {
            new Regex(@"[0-9]", RegexOptions.Compiled),
            new Regex(@"[a-z]", RegexOptions.Compiled),
            new Regex(@"[A-Z]", RegexOptions.Compiled),
            new Regex(@"[\x20-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]", RegexOptions.Compiled),
        }.AsReadOnly();
    }

    // validity check methods remain unchanged
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

    public static byte[] GetRandomSalt()
    {
        byte[] salt = new byte[SALT_SIZE];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    // New password hashing method
    public static string HashPassword(string password)
    {
        byte[] salt = GetRandomSalt();

        byte[] hash = GetPbkdf2Bytes(password, salt, PBKDF2_ITERATIONS, HASH_SIZE);

        byte[] hashBytes = new byte[SALT_SIZE + HASH_SIZE];
        Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE);
        Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);

        return Convert.ToBase64String(hashBytes);
    }

    // New password verification method
    public static bool VerifyPassword(string? password, string? hashedPassword)
    {
        if (password == null || hashedPassword == null)
            return false;

        byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        if (hashBytes.Length != SALT_SIZE + HASH_SIZE)
            return false;

        byte[] salt = new byte[SALT_SIZE];
        Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE);

        byte[] expectedHash = new byte[HASH_SIZE];
        Array.Copy(hashBytes, SALT_SIZE, expectedHash, 0, HASH_SIZE);

        byte[] actualHash = GetPbkdf2Bytes(password, salt, PBKDF2_ITERATIONS, HASH_SIZE);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    public static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(outputBytes);
    }
}