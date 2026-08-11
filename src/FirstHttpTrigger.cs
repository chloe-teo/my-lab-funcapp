using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyLabFuncApp;

public class FirstHttpTrigger
{
    private readonly ILogger<FirstHttpTrigger> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public FirstHttpTrigger(ILogger<FirstHttpTrigger> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

        [Function("first_http_function")]
    public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "api/first_http_function")] HttpRequestData req)
    {
        _logger.LogInformation("first_http_function function processed a request.");

        // Build base URI from the incoming request
        // Take the full incoming URL as string
        var incomingUrl = req.Url.AbsoluteUri;
        // Split at "/api/" and rebuild the base
        var baseUrl = $"{incomingUrl.Split("/api/")[0]}/api";

        // Append the known route of the second function
        var targetUri = $"{baseUrl}/second_http_function";


        // Create HttpClient from factory
        var client = _httpClientFactory.CreateClient();

        // Example call to FunctionB
        var response = await client.GetAsync(targetUri);
        var content = await response.Content.ReadAsStringAsync();

        return new OkObjectResult($"Called second_http_function, status: {response.StatusCode}, content: {content}");
    }
}