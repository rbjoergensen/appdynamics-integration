using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDynTestBackend.Helpers;
using AppDynTestBackend.Models;
using AppDynTestBackend.Data;

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
            Logging.log.Information("weatherforecast endpoint called");

            var rng = new Random();

            var Summaries = Data.Sql.GetWeatherTypes();

            return Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("/api/v1/weathertype")]
        public ActionResult<Status> PostAlertConfiguration([FromBody] TestData body)
        {
            Logging.log.Information("weathertype endpoint called");
            try
            {
                if (body == null)
                {
                    Logging.log.Error($"/api/v1/weathertype: body is null");

                    return StatusCode(400, new Status { Result = false, Message = "body is null" });
                }
                if (body.WeatherType == null)
                {
                    Logging.log.Error($"/api/v1/config: WeatherType missing from body");

                    return StatusCode(400, new Status { Result = false, Message = "WeatherType missing from body" });
                }
                return StatusCode(201, Sql.AddWeatherType(body));
            }
            catch (Exception ex)
            {
                Logging.log.Error(ex.Message);
                return StatusCode(500, new Status { Result = false, Message = ex.Message });
            }
        }
    }
}
