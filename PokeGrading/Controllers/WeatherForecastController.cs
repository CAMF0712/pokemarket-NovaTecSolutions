// Controlador HTTP: coordina el flujo de entrada/salida para WeatherForecastController.
using Microsoft.AspNetCore.Mvc;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    /// <summary>
    /// Clase principal que concentra la responsabilidad de WeatherForecastController en esta capa.
    /// </summary>
    public class WeatherForecastController : ControllerBase
    {
        private const int WeatherForecastStartDayOffset = 1;
        private const int WeatherForecastDaysCount = 5;
        private const int MinimumTemperatureCelsius = -20;
        private const int MaximumTemperatureCelsius = 55;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de WeatherForecastController.
        /// </summary>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        /// <summary>
        /// Devuelve una lista de pronosticos de ejemplo para validar el endpoint base.
        /// </summary>
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(
                WeatherForecastStartDayOffset,
                WeatherForecastDaysCount)
            .Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(
                    MinimumTemperatureCelsius,
                    MaximumTemperatureCelsius),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

