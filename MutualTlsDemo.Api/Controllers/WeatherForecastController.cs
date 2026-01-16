using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualTlsDemo.Api.AuthenticationAlternatives;

namespace MutualTlsDemo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
  private readonly VayapayClient _vayapayClient;

  public WeatherForecastController(VayapayClient vayapayClient)
  {
    _vayapayClient = vayapayClient;
  }

  private static readonly string[] Summaries =
  [
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
  ];

  [HttpGet(Name = "GetWeatherForecast")]
  [Authorize(AuthenticationSchemes = VayapayAuthenticationHandlerScheme.AuthenticationScheme)]
  public IEnumerable<WeatherForecast> Get()
  {
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
          Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          TemperatureC = Random.Shared.Next(-20, 55),
          Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();
  }

  [HttpPost]
  public async Task<IActionResult> PostAsync()
  {
    var result = await _vayapayClient.GetDataAsync(); // Example usage of the injected HttpClient
    Console.WriteLine("Response from Vayapay API:");
    Console.WriteLine(result);

    return Ok("POST request received.");
  }
}
