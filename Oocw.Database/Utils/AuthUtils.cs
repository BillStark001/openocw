using System;
using System.Security.Cryptography;
using System.Text;

namespace Oocw.Database.Utils;


public static class AuthUtils
{

    public const int CHALLENGE_SIZE = 32;

    public static string GenerateChallenge()
    {
        byte[] challengeBytes = new byte[CHALLENGE_SIZE];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(challengeBytes);
        }
        return Convert.ToBase64String(challengeBytes);
    }

    public static string CreateChallengeResponse(string challenge, string password)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(password));
        byte[] challengeBytes = Convert.FromBase64String(challenge);
        byte[] responseBytes = hmac.ComputeHash(challengeBytes);
        return Convert.ToBase64String(responseBytes);
    }

    public static bool VerifyChallengeResponse(string challenge, string response, string storedPasswordHash)
    {
        // extract salt
        byte[] hashBytes = Convert.FromBase64String(storedPasswordHash);
        byte[] salt = new byte[UserUtils.SALT_SIZE];
        Array.Copy(hashBytes, 0, salt, 0, UserUtils.SALT_SIZE);

        // regenerate hashed bytes
        byte[] passwordBytes = UserUtils.GetPbkdf2Bytes(storedPasswordHash, salt, UserUtils.PBKDF2_ITERATIONS, UserUtils.HASH_SIZE);

        // create hmac
        using var hmac = new HMACSHA256(passwordBytes);
        byte[] challengeBytes = Convert.FromBase64String(challenge);
        byte[] expectedResponseBytes = hmac.ComputeHash(challengeBytes);
        byte[] actualResponseBytes = Convert.FromBase64String(response);

        return CryptographicOperations.FixedTimeEquals(expectedResponseBytes, actualResponseBytes);
    }
}