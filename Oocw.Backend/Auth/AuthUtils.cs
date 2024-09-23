using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.AspNetCore.DataProtection;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Oocw.Database.Models;
using Oocw.Backend.Controllers;
using Oocw.Backend.Models;
using Oocw.Backend.Services;
using Microsoft.AspNetCore.Http;

namespace Oocw.Backend.Auth;


public static class AuthUtils
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public const string KEY_UPDATED_AT = "updated_at";
    public const string KEY_USAGE = "usage";
    public const string KEY_ITEM_USER = "user";

    public static string GenerateRefreshToken(this User user, JwtConfig config)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Aud, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(KEY_UPDATED_AT, user.RefreshTime.ToBinary().ToString())
            ]),

            Expires = DateTime.UtcNow.AddDays(config.RefreshExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = TokenHandler.WriteToken(token);

        return jwtToken;
    }

    public static string GenerateAccessToken(this User user, string? accessType, JwtConfig config)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);
        accessType = accessType ?? "";

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Aud, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(KEY_USAGE, accessType)
            ]),

            Expires = DateTime.UtcNow.AddMinutes(config.AccessExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = TokenHandler.WriteToken(token);

        return jwtToken;
    }

    public static bool VerifyRefreshToken(string tokenRaw, JwtConfig config, DatabaseService dbs, out User? user)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        try
        {
            var token = TokenHandler.ValidateToken(tokenRaw, validations, out var tokenValidated);
            user = dbs.Wrapper.FindUser(int.Parse(token.FindFirstValue(JwtRegisteredClaimNames.Aud)));
            if (user == null)
                return false;
            if (user.RefreshTime > DateTime.FromBinary(long.Parse(token.FindFirstValue(KEY_UPDATED_AT))))
                return false; // a force logout is triggered due to pwd change, etc.
            return true;
        }
        catch (Exception)
        {
            user = null;
            return false;
        }
    }

    public static bool VerifyAccessToken(string tokenRaw, JwtConfig config, string? accessType, DatabaseService dbs, out User? user)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        user = null;
        try
        {
            var token = TokenHandler.ValidateToken(tokenRaw, validations, out var tokenValidated);
            if (token.FindFirstValue(KEY_USAGE) != (accessType ?? ""))
                return false;
            user = dbs.Wrapper.FindUser(int.Parse(token.FindFirstValue(JwtRegisteredClaimNames.Aud)));
            return user != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static User? GetUser(this HttpContext context) {
        return context.Items[KEY_ITEM_USER] as User;
    }
}