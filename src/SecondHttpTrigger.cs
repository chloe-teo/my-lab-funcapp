using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyLabFuncApp;


public class SecondHttpTrigger
{
    private readonly ILogger<SecondHttpTrigger> _logger;

    public SecondHttpTrigger(ILogger<SecondHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("second_http_function")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "api/second_http_function")] HttpRequestData req)
    {
        _logger.LogInformation("second_http_function function processed a request.");

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.WriteStringAsync("Hello from second_http_function!");

        return response;
    }
}