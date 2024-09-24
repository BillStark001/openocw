using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Oocw.Backend.Models;
using Oocw.Backend.Services;
using Oocw.Backend.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace Oocw.Backend.Auth;

public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtConfig _jwtConfig;
    private readonly DatabaseService _dbService;

    public JwtAuthMiddleware(RequestDelegate next, IOptions<JwtConfig> jwtConfig, DatabaseService dbService)
    {
        _next = next;
        _jwtConfig = jwtConfig.Value;
        _dbService = dbService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var authorizeAttribute = endpoint?.Metadata.GetMetadata<RequireAuthAttribute>();
        if (authorizeAttribute == null) {
            // authorization is not needed
            await _next(context);
            return;
        }

        var accessType = authorizeAttribute.AccessType;

        var fullAccessType = string.IsNullOrEmpty(accessType)
            ? Definitions.KEY_ACCESS_TOKEN
            : string.Format(Definitions.KEY_ACCESS_TOKEN_FORMAT, accessType);

        if (context.Request.Cookies.TryGetValue(fullAccessType, out var accessToken))
        {
            var user = await _dbService.VerifyAccessTokenAsync(accessToken, _jwtConfig, accessType);
            if (user != null)
            {
                context.Items[TokenUtils.KEY_ITEM_USER] = user;
                await _next(context);
                return;
            }
        }

        if (context.Request.Cookies.TryGetValue(Definitions.KEY_REFRESH_TOKEN, out var refreshToken))
        {
            var user = await _dbService.VerifyRefreshTokenAsync(refreshToken, _jwtConfig);
            if (user != null)
            {
                var newAccessToken = TokenUtils.GenerateAccessToken(user!, accessType, _jwtConfig);
                context.Response.Cookies.Append(fullAccessType, newAccessToken);
                context.Items[TokenUtils.KEY_ITEM_USER] = user;
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new StandardResult(Definitions.CODE_ERR_AUTH_FAILED));
    }
}