using System.Diagnostics;
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
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/second_http_function")] HttpRequestData req)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("function.name", "second_http_function");
            activity.SetTag("function.trigger", "http");
        }

        _logger.LogInformation("second_http_function function processed a request.");

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.WriteStringAsync("Hello from second_http_function!");

        return response;
    }
}