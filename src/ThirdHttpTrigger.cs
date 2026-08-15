using System.Diagnostics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyLabFuncApp;

public class ThirdHttpTrigger
{
    private readonly ILogger<ThirdHttpTrigger> _logger;

    public ThirdHttpTrigger(ILogger<ThirdHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("third_http_function")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/third_http_function")] HttpRequestData req)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("function.name", "third_http_function");
            activity.SetTag("function.trigger", "http");
        }

        _logger.LogInformation("third_http_function function processed a request.");

        // Simulate an exception for testing telemetry
        throw new InvalidOperationException("This is a simulated exception from third_http_function to test telemetry and exception tracking.");
    }
}
