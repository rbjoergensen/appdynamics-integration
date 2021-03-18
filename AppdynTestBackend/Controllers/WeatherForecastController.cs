using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using AppDynTestBackend.Models;

namespace AppDynTestBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/api/v1/weatherforecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("weatherforecast endpoint called");

            var rng = new Random();

            var summaries = Sql.GetWeatherTypes();

            return Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = summaries[rng.Next(summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("/api/v1/weathertype")]
        public ActionResult<Status> PostAlertConfiguration([FromBody] TestData body)
        {
            _logger.LogInformation("weathertype endpoint called");
            try
            {
                if (body == null)
                {
                    _logger.LogError($"/api/v1/weathertype: body is null");

                    return StatusCode(400, new Status { Result = false, Message = "body is null" });
                }

                if (!string.IsNullOrEmpty(body.WeatherType)) return StatusCode(201, Sql.AddWeatherType(body));
                
                _logger.LogError($"/api/v1/weathertype: WeatherType missing from body");
                
                return StatusCode(400, new Status { Result = false, Message = "WeatherType missing from body" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new Status { Result = false, Message = ex.Message });
            }
        }
    }
}
