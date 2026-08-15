using System.Net;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyLabFuncApp;

public class KeyVaultHttpTrigger
{
    private readonly ILogger<KeyVaultHttpTrigger> _logger;
    private readonly SecretClient _secretClient;
    private readonly string _secretName;

    public KeyVaultHttpTrigger(ILogger<KeyVaultHttpTrigger> logger)
    {
        _logger = logger;

        var vaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI")
            ?? throw new InvalidOperationException("KEY_VAULT_URI is not configured.");
        _secretName = Environment.GetEnvironmentVariable("KEY_VAULT_TEST_SECRET_NAME")
            ?? throw new InvalidOperationException("KEY_VAULT_TEST_SECRET_NAME is not configured.");

        _secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
    }

    [Function("keyvault_test")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/keyvault_test")] HttpRequestData req)
    {
        try
        {
            var secret = await _secretClient.GetSecretAsync(_secretName);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                secretName = secret.Value.Name,
                value = secret.Value.Value
            });
            return response;
        }
        catch (Exception exception) when (exception is Azure.RequestFailedException or UriFormatException)
        {
            _logger.LogError(exception, "Key Vault test read failed for secret {SecretName}.", _secretName);
            var response = req.CreateResponse(HttpStatusCode.BadGateway);
            await response.WriteAsJsonAsync(new { error = "Key Vault read failed." });
            return response;
        }
    }
}