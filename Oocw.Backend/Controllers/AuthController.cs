using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oocw.Backend.Models;
using Oocw.Backend.Services;
using Oocw.Database;
using Oocw.Database.Models;
using Oocw.Backend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Definitions = Oocw.Backend.Models.Definitions;
using Oocw.Utils;
using Oocw.Backend.Auth;
using Oocw.Database.Utils;
using Oocw.Database.Models.Technical;
using System.Threading.Tasks;
using Oocw.Backend.Api;

namespace Oocw.Backend.Controllers;

public class UnamePwdBody
{
    public string uname { get; set; } = null!;
    public string pwd { get; set; } = null!;
}

public class UnameBody
{
    public string uname { get; set; } = null!;
}

[ApiController]
[Route("/api/user")]
public class AuthController : Controller
{
    [FromServices] public DatabaseService DbService { get; set; } = null!;
    [FromServices] public IOptions<JwtConfig> JwtConfig { get; set; } = null!;
    

    [HttpPost("register")]
    public async Task Register(UnamePwdBody b)
    {
        if (!UserUtils.IsValidUsername(b.uname))
            throw new ApiException(Definitions.CODE_ERR_INVALID_UNAME);
        if (!UserUtils.IsValidPassword(b.pwd))
            throw new ApiException(Definitions.CODE_ERR_INVALID_PWD);

        try
        {
            await DbService.Wrapper.CreateUserAsync(b.uname, UserUtils.HashPassword(b.pwd));
        }
        catch (UserNameConflictException)
        {
            throw new ApiException(Definitions.CODE_ERR_UNAME_CONFLICT);
        }
        catch (DatabaseInternalException)
        {
            throw new ApiException(Definitions.CODE_ERR_DB_ERR);
        }
        // successful, do nothing
    }

    [HttpPost("auth")]
    public AuthResult Auth(UnamePwdBody b)
    {
        var u = DbService.Wrapper.QueryUser(b.uname);
        if (u == null || !UserUtils.VerifyPassword(b.pwd, u.PasswordEncrypted))
            throw new ApiException(Definitions.CODE_ERR_BAD_UNAME_OR_PWD);

        var token = u.GenerateRefreshToken(JwtConfig.Value);

        return new AuthResult(token);
    }

    [HttpPost("login")]
    public AuthResult Login(UnamePwdBody b)
    {
        var val = Auth(b);

        Response.Cookies.Append(Definitions.KEY_REFRESH_TOKEN, val.Token);

        return val;
    }

    [HttpPost("forget")]
    public void ForgetPassword(UnameBody b)
    {
        var u = DbService.Wrapper.QueryUser(b.uname) ?? throw new ApiException(Definitions.CODE_ERR_INVALID_UNAME);
        throw new NotImplementedException();
    }

    [RequireAuth]
    [HttpGet("status")]
    public void Check()
    {
        // do nothing
    }

    [RequireAuth]
    [HttpPost("logout")]
    public void LogOut()
    {
        Response.Cookies.Delete(Definitions.KEY_REFRESH_TOKEN);
        Response.Cookies.Delete(Definitions.KEY_ACCESS_TOKEN);
    }
}