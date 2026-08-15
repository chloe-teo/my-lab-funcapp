using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MyLabFuncApp;

public class LandingPage
{
    [Function("landing_page")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "index")] HttpRequestData req)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "assets", "index.html");
        if (!File.Exists(path))
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        await response.WriteStringAsync(await File.ReadAllTextAsync(path));
        return response;
    }

      [Function("landing_asset")]
      public async Task<HttpResponseData> Asset([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assets/{fileName}")] HttpRequestData req, string fileName)
      {
        var allowedFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
          ["func-app-architecture.png"] = "image/png",
          ["azure-monitor-workbook.png"] = "image/png",
          ["terraform-modules.png"] = "image/png"
        };

        if (!allowedFiles.TryGetValue(fileName, out var contentType))
        {
          return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var path = Path.Combine(AppContext.BaseDirectory, "assets", fileName);
        if (!File.Exists(path))
        {
          return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", contentType);
        await response.Body.WriteAsync(await File.ReadAllBytesAsync(path));
        return response;
      }
}