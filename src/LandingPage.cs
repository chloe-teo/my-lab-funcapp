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
                body { margin: 0; min-height: 100vh; display: grid; place-items: center; color: #f7f2e8; background: #17252a; }
                main { width: min(680px, 82vw); padding: 64px 0; }
                .eyebrow { color: #f2a65a; letter-spacing: .16em; text-transform: uppercase; font: 700 40px system-ui, sans-serif; }
                h1 { max-width: 620px; margin: 18px 0; font-size: clamp(48px, 10vw, 92px); line-height: .92; font-weight: 400; }
                p { max-width: 480px; color: #bed0cc; font: 18px/1.6 system-ui, sans-serif; }
                nav { display: flex; flex-wrap: wrap; gap: 12px; margin-top: 36px; }
                a { padding: 12px 16px; border: 1px solid #54716f; color: #f7f2e8; text-decoration: none; font: 600 14px system-ui, sans-serif; }
                a:hover { border-color: #f2a65a; color: #f2a65a; }
              </style>
            </head>
            <body><main>
              <div class="eyebrow">Welcome to Chloe's My Lab Functions</div>
              <h2>Small functions.<br>Useful experiments.</h2>
              <p>An end-to-end lab for provisioning Azure infrastructure with Terraform, deploying a .NET function app, and observing it with OpenTelemetry.</p>
              <nav><a href="/api/first_http_function">Try the first function</a><a href="/api/second_http_function">Try the second function</a></nav>
            </main></body>
            </html>
            """);
        return response;
    }
}