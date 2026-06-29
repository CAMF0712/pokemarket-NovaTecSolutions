namespace PokeGrading
{
    public class WeatherForecast
    {
        private const int FreezingPointFahrenheit = 32;
        private const double CelsiusToFahrenheitDivisor = 0.5556;

        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF =>
            FreezingPointFahrenheit +
            (int)(TemperatureC / CelsiusToFahrenheitDivisor);

        public string? Summary { get; set; }
    }
}
