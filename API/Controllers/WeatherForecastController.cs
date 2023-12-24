using Microsoft.AspNetCore.Mvc; // Importe le namespace de Microsoft.AspNetCore.Mvc

namespace API.Controllers; // Déclare un namespace pour le contrôleur

[ApiController] 
[Route("[controller]")] 
public class WeatherForecastController : BaseApiController // Hérite de ControllerBase pour utiliser les fonctionnalités d'ASP.NET Core MVC
{
    private static readonly string[] Summaries = new[] // Initialise un tableau de chaînes contenant des descriptions de températures
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger; // Déclare un champ privé pour stocker l'enregistreur de journalisation

    public WeatherForecastController(ILogger<WeatherForecastController> logger) // Initialise un nouveau contrôleur avec un enregistreur de journalisation
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")] 
    public IEnumerable<WeatherForecast> Get() 
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast 
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)), // Définit la date de chaque prévision
            TemperatureC = Random.Shared.Next(-20, 55), // Génère une température aléatoire en degrés Celsius
            Summary = "Hello word" // Définit un résumé pour chaque prévision
        })
        .ToArray(); // Convertit la liste en tableau et la retourne
    }
}
