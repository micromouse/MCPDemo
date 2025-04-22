using AspNetcoreSSEServer.Application.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetcoreSSEServer.Controllers {
    /// <summary>
    /// Weather forecast controller
    /// </summary>
    /// <param name="logger">ÈÕÖ¾Æ÷</param>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase {
        private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        /// <summary>
        /// Get weather forecast
        /// </summary>
        /// <returns>weather forecast</returns>
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecastViewModel> Get() {
            logger.LogInformation("GetWeatherForecast called at {time}", DateTime.Now);

            return [.. Enumerable.Range(1, 5).Select(index => new WeatherForecastViewModel {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = _summaries[Random.Shared.Next(_summaries.Length)]
            })];
        }
    }
}
