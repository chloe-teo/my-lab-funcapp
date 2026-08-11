using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MyLabFuncApp;

public class LandingPage
{
    [Function("landing_page")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "index")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        await response.WriteStringAsync("""
            <!doctype html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>My Lab Functions</title>
              <style>
                :root { color-scheme: dark; font-family: Georgia, serif; }
                body { margin: 0; min-height: 100vh; color: #f7f2e8; background: #17252a; }
                main { width: min(980px, 88vw); margin: 0 auto; padding: 42px 0 56px; }
                .eyebrow { color: #f2a65a; letter-spacing: .16em; text-transform: uppercase; font: 700 40px system-ui, sans-serif; }
                h2 { max-width: 620px; margin: 18px 0; font-size: clamp(48px, 10vw, 92px); line-height: .92; font-weight: 400; }
                p { max-width: 480px; color: #bed0cc; font: 18px/1.6 system-ui, sans-serif; }
                nav { display: flex; flex-wrap: wrap; gap: 12px; margin-top: 36px; }
                a { padding: 12px 16px; border: 1px solid #54716f; color: #f7f2e8; text-decoration: none; font: 600 14px system-ui, sans-serif; }
                a:hover { border-color: #f2a65a; color: #f2a65a; }
                .gallery { display: grid; grid-template-columns: minmax(0, 1fr); gap: 42px; margin-top: 48px; }
                figure { margin: 0; }
                img { display: block; width: 100%; aspect-ratio: 4 / 3; object-fit: contain; background: #f7f2e8; border: 1px solid #54716f; }
                figcaption { margin-top: 14px; color: #bed0cc; font: 16px/1.55 system-ui, sans-serif; }
              </style>
            </head>
            <body><main>
              <div class="eyebrow">Welcome to Chloe's My Lab Functions</div>
              <h3>Small functions.<br>Useful experiments.</h3>
              <p>An end-to-end lab for provisioning Azure infrastructure with Terraform, deploying a .NET function app, and observing it with OpenTelemetry.</p>
              <section class="gallery" aria-label="Architecture diagrams">
                <figure><img src="/assets/func-app-architecture.png" alt="Function app architecture diagram"><figcaption>This diagram shows how the Azure Function App connects the from code to deployment pipeline, the azure resource component used in this project for a secured Azure Function app within private network and telemetry send to Azure App Insights.</figcaption></figure>
                <figure><img src="/assets/terraform-modules.png" alt="Terraform module resource map"><figcaption>This map shows the Azure resources created and connected by the Terraform module. It makes the infrastructure boundaries and deployment relationships easier to understand.</figcaption></figure>
              </section>
              <nav><a href="/api/first_http_function">Try the first function</a><a href="/api/second_http_function">Try the second function</a></nav>
            </main></body>
            </html>
            """);
        return response;
    }

      [Function("landing_asset")]
      public async Task<HttpResponseData> Asset([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assets/{fileName}")] HttpRequestData req, string fileName)
      {
        var allowedFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
          ["func-app-architecture.png"] = "image/png",
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