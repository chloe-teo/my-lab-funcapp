using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Logging.AddFilter("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogLevel.None);
builder.Logging.AddFilter("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogLevel.None);
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.None);
builder.Logging.AddFilter("System.Net.Http.HttpClient.Default", LogLevel.None);
builder.Logging.AddFilter("System.Net.Http.HttpClient.CallInvokerExtractor", LogLevel.None);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()    
    .WithTracing(tracing =>
    {
        tracing.AddHttpClientInstrumentation();
    });

// Get the client ID for the user-assigned managed identity from environment variables
var clientId = Environment.GetEnvironmentVariable("AzureWebJobsStorage__clientId");

var credentialOptions = new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = clientId
};

var credential = new DefaultAzureCredential(credentialOptions);

builder.Services.AddOpenTelemetry().UseAzureMonitorExporter(options =>
{
    options.Credential = credential;
});
builder.Services.AddOpenTelemetry().UseFunctionsWorkerDefaults();

// To send telemetry to an OTLP endpoint, uncomment the following line and add OTEL_EXPORTER_OTLP_ENDPOINT and OTEL_EXPORTER_OTLP_HEADERS environment variables
//builder.Services.AddOpenTelemetry().UseOtlpExporter();

builder.Build().Run();
