using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace ApiRetry.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController : ControllerBase
{
    private readonly ILogger<ServiceRequestsController> _logger;


    public ServiceRequestsController(ILogger<ServiceRequestsController> logger)
    {
        _logger = logger;
    }


    [HttpGet(Name = "GetData")]
    public async Task<IActionResult> Get()
    {
        await ConnectToApi();
        return Ok();
    }


    private async Task ConnectToApi()
    {
        var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";

        var client = new RestClient(url);

        var request = new RestRequest(url, Method.Get);

        request.AddHeader("accept", "application/json");
        request.AddHeader("X-RapidAPI-Key", "11bf24e56emsh300f1cf74a59919p1d4ef9jsn95d784614af2");
        request.AddHeader("X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            Console.WriteLine(response.Content);
        }
        else
        {
            Console.WriteLine(response.ErrorMessage);
        }
    }

}