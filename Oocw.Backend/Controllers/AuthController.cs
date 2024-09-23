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
    public ActionResult<StandardResult> Register(UnamePwdBody b)
    {
        if (!UserUtils.IsValidUsername(b.uname))
            return BadRequest(new StandardResult(Definitions.CODE_ERR_INVALID_UNAME, b.uname));
        if (!UserUtils.IsValidPassword(b.pwd))
            return BadRequest(new StandardResult(Definitions.CODE_ERR_INVALID_PWD, b.pwd));

        try
        {
            DbService.Wrapper.Register(b.uname, b.pwd);
        }
        catch (UserNameConflictException)
        {
            return BadRequest(new StandardResult(Definitions.CODE_ERR_UNAME_CNFL, b.uname));
        }
        catch (DatabaseInternalException e)
        {
            return BadRequest(new StandardResult(Definitions.CODE_ERR_DB_ERR, e));
        }

        return new StandardResult(Definitions.CODE_SUCC);
    }

    [HttpPost("auth")]
    public ActionResult<StandardResult> Auth(UnamePwdBody b)
    {
        var u = DbService.Wrapper.QueryUser(b.uname);
        if (u == null || !UserUtils.verifyPassword(b.pwd, u.PasswordEncrypted))
            return new StandardResult(Definitions.CODE_ERR_BAD_UNAME_OR_PWD);

        // TODO add additional security measures
        var token = u.GenerateRefreshToken(JwtConfig.Value);

        return new AuthResult(token);
    }

    [HttpPost("login")]
    public ActionResult<StandardResult> Login(UnamePwdBody b)
    {
        var res = Auth(b);
        var val = res.Value;
        if (val == null || val is not AuthResult || val.Code != Definitions.CODE_SUCC.Item1)
            return res; // auth failed

        Response.Cookies.Append(Definitions.KEY_REFRESH_TOKEN, ((AuthResult)val).Token);

        return new StandardResult(Definitions.CODE_SUCC);
    }

    [HttpPost("forget")]
    public ActionResult<StandardResult> ForgetPassword(UnameBody b)
    {
        var u = DbService.Wrapper.QueryUser(b.uname);
        if (u == null)
            return new StandardResult(Definitions.CODE_ERR_INVALID_UNAME, b.uname);

        throw new NotImplementedException();
    }

    [RequireAuth]
    [HttpGet("status")]
    public ActionResult<StandardResult> Check()
    {
        return new StandardResult(Definitions.CODE_SUCC);
    }

    [RequireAuth]
    [HttpPost("logout")]
    public ActionResult<StandardResult> LogOut()
    {
        Response.Cookies.Delete(Definitions.KEY_REFRESH_TOKEN);
        Response.Cookies.Delete(Definitions.KEY_ACCESS_TOKEN);
        return new StandardResult(Definitions.CODE_SUCC);
    }
}