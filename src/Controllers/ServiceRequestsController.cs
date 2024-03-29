using Microsoft.AspNetCore.Mvc;
using Polly;
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
        await ConnectToApiAsync();
        return Ok();
    }

    [HttpGet("GetDataWithRetry")]
    public async Task<IActionResult> GetDataWithRetryAsync()
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .RetryAsync(5, onRetry: (exception, retryCount) => 
            {
                Console.WriteLine($"Error: {exception.Message} ... Retry Count: {retryCount}");
            });

        await retryPolicy.ExecuteAsync(async () => 
        {
            await ConnectToApiAsync();
        });
        
        return Ok();
    }

    [HttpGet("GetDataWithWaitAndRetry")]
    public async Task<IActionResult> GetDataWithWaitAndRetryAsync()
    {
        var amountToPause = TimeSpan.FromSeconds(15);

        var retryWaitPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, t => amountToPause, onRetry: (exception, retryCount) => 
            {
                Console.WriteLine($"Error: {exception.Message} ... Wait Next Retry: {retryCount}");
            });

        await retryWaitPolicy.ExecuteAsync(async () => 
        {
            await ConnectToApiAsync();
        });

        return Ok();
    }


    [HttpGet("GetDataWithCircuitBreaker")]
    public async Task<IActionResult> GetDataWithCircuitBreakerSync()
    {
        var amountToPause = TimeSpan.FromSeconds(15);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, i => amountToPause, (exception, retryCount) => 
            {
                Console.WriteLine($"Error: {exception.Message} ... Wait Next Retry: {retryCount}");
            });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
                onBreak: (ex, breakDelay) => 
                {
                    Console.WriteLine($"{"[Break]",-10}{breakDelay,-10:ss\\.fff}: {ex.GetType().Name}");
                },
                onReset: () => 
                {
                    Console.WriteLine($"{"[Reset]",-10}");
                },
                onHalfOpen: () => 
                {
                    Console.WriteLine($"{"[HalfOpen]",-10}");
                });

        var finalPolicy = retryPolicy.WrapAsync(circuitBreakerPolicy);

        await finalPolicy.ExecuteAsync(async () => 
        {
            Console.WriteLine("Executing");
            await ConnectToApiAsync();
        });

        return Ok();
    }
    

    private async Task ConnectToApiAsync()
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
            throw new Exception("Not able to connect to the service");
        }
    }

    private void ConnectToApi()
    {
        var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";

        var client = new RestClient(url);

        var request = new RestRequest(url, Method.Get);

        request.AddHeader("accept", "application/json");
        request.AddHeader("X-RapidAPI-Key", "11bf24e56emsh300f1cf74a59919p1d4ef9jsn95d784614af2");
        request.AddHeader("X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");

        var response = client.Execute(request);

        if (response.IsSuccessful)
        {
            Console.WriteLine(response.Content);
        }
        else
        {
            Console.WriteLine(response.ErrorMessage);
            throw new Exception("Not able to connect to the service");
        }
    }

}