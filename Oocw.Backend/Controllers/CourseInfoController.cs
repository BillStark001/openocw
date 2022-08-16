using Microsoft.AspNetCore.Mvc;
using Oocw.Backend.Schemas;

namespace Oocw.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseInfoController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<CourseInfoController> _logger;

        public CourseInfoController(ILogger<CourseInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/api/course/info/{id}")]
        public string Info(string id, int? year, string? className)
        {
            return $"## Test Markdown\n### {id} / {year} / {className}";
        }

        [HttpGet("/api/course/brief/{id}")]
        public CourseBrief Brief(string id, int? year, string? className)
        {
            return new()
            {
                Id = id,
                Name = "",
                ClassName = className
            };
        }

        [HttpGet("/api/course/search")]
        public IEnumerable<CourseBrief> Search(string query, string? restrictions, int? dispCount, int? page)
        {
            throw new NotImplementedException();
        }


    }
}