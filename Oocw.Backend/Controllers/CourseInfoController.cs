using Microsoft.AspNetCore.Mvc;

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

        [HttpGet(Name = "/course/info/{id}?year={year}&class={className}")]
        public IEnumerable<WeatherForecast> Get(string id, int? year, string? className)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}