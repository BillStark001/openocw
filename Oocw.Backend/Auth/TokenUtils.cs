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
using System.Threading.Tasks;
using MongoDB.Driver;
using Oocw.Database.Models.Technical;

namespace Oocw.Backend.Auth;


public static class TokenUtils
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public const string KEY_UPDATED_AT = "updated_at";
    public const string KEY_USAGE = "usage";
    public const string KEY_ITEM_USER = "user";

    public static string GenerateRefreshToken(this User user, JwtConfig config)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);
        var updateTime = user.UpdateTime != null
            ? user.UpdateTime.Value.ToBinary().ToString()
            : "";
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Aud, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UpdatedAt, updateTime)
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

    public static async Task<User?> VerifyRefreshTokenAsync(this DatabaseService dbs, string tokenRaw, JwtConfig config)
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
            var userId = token.FindFirstValue(JwtRegisteredClaimNames.Aud);
            var user = await dbs.Wrapper.Users.FindByIdAsync(null, userId);
            if (user != null && user.UpdateTime > DateTime.FromBinary(long.Parse(token.FindFirstValue(JwtRegisteredClaimNames.UpdatedAt) ?? "0")))
                return null; // a force logout is triggered due to pwd change, etc.
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<User?> VerifyAccessTokenAsync(this DatabaseService dbs, string tokenRaw, JwtConfig config, string? accessType)
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
            if (token.FindFirstValue(KEY_USAGE) != (accessType ?? ""))
                return null;
            var user = await dbs.Wrapper.Users.FindByIdAsync(null, token.FindFirstValue(JwtRegisteredClaimNames.Aud));
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static User? GetUser(this HttpContext context) {
        return context.Items[KEY_ITEM_USER] as User;
    }
}