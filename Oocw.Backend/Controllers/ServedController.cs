using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oocw.Backend.Services;
using System;

namespace Oocw.Backend.Controllers;

[ApiController]
public class ServedController : ControllerBase
{
    protected readonly ILogger<AuthController> _logger;
    protected readonly DatabaseService _dbService;
    protected readonly JwtConfig _jwtConfig;


    public DatabaseService DBService => _dbService;
    public JwtConfig JwtConfig => _jwtConfig;

    public ServedController(
        ILogger<AuthController> logger,
        DatabaseService service,
        IOptions<JwtConfig> jwtConfig
        )
    {
        _logger = logger;
        _dbService = service;
        _jwtConfig = jwtConfig.Value;
    }
}
