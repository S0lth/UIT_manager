using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace UITManagerApi.Controllers {
    [ApiController]
    [ApiVersion(1.0, Deprecated = true)]
    [ApiVersion(1.1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger) {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast"), MapToApiVersion(1.0)]
        public IEnumerable<WeatherForecast> Get() {
            return GenerateWeatherForecastFor(5);
        }

        [HttpGet(Name = "GetV2WeatherForecast"), MapToApiVersion(1.1)]
        public IEnumerable<WeatherForecast> GetV2() {
            return GenerateWeatherForecastFor(7);
        }
        
        [HttpGet]
        public IEnumerable<WeatherForecast> GenerateWeatherForecastFor(int days) {
            return Enumerable.Range(1, days).Select(index => new WeatherForecast {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
