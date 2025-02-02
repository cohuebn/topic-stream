using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using TopicStream.FunctionalTests.Configuration;

namespace TopicStream.FunctionalTests.ApiKeys;

/// <summary>
/// Responsible for provisioning temporary API keys that can be used by all tests.
/// After all tests finish, the API keys are deleted.
/// </summary>
public class ApiKeyProvisioner : IAsyncLifetime
{
  private readonly AmazonAPIGatewayClient _apiGatewayClient;
  private readonly EasyRetry.EasyRetry _retrier;
  public TestApiKeys? ApiKeys { get; private set; }

  public ApiKeyProvisioner()
  {
    _apiGatewayClient = new AmazonAPIGatewayClient();
    // Retrier used to prevent AWS issues from causing test failures (e.g. throttling failures)
    _retrier = new EasyRetry.EasyRetry();
  }

  private async Task<string> CreateApiKey(string name, string value)
  {
    var response = await _retrier.Retry(() =>
      {
        return _apiGatewayClient.CreateApiKeyAsync(new CreateApiKeyRequest
        {
          Name = name,
          Value = value,
          Enabled = true,
        });
      });
    return response.Id;
  }

  private async Task DeleteApiKey(string apiKeyIdentifier)
  {
    await _retrier.Retry(() =>
      {
        return _apiGatewayClient.DeleteApiKeyAsync(new DeleteApiKeyRequest
        {
          ApiKey = apiKeyIdentifier,
        });
      });
  }

  /// <summary>
  /// Creates API keys for testing and stores their identifiers
  /// </summary>
  /// <returns>A task that completes after keys are created</returns>
  public async ValueTask InitializeAsync()
  {
    // Timestamp added to key name just to troubleshoot if API keys linger between tests
    var timestamp = DateTime.UtcNow.ToString("u");
    var apiKeyNameValuePairs = new[]
    {
      new { Name = $"Subscriber1-{timestamp}", Value = TestConfiguration.GetSubscriber1ApiKey() },
      new { Name = $"Subscriber2-{timestamp}", Value = TestConfiguration.GetSubscriber2ApiKey() },
      new { Name = $"Publisher-${timestamp}", Value = TestConfiguration.GetPublisherApiKey() }
    };
    var apiKeys = await Task.WhenAll(apiKeyNameValuePairs.Select(async apiKeyNameValuePair =>
    {
      var apiKeyId = await CreateApiKey(apiKeyNameValuePair.Name, apiKeyNameValuePair.Value);
      return new TestApiKey(apiKeyId, apiKeyNameValuePair.Name, apiKeyNameValuePair.Value);
    }));
    ApiKeys = new TestApiKeys(apiKeys[0], apiKeys[1], apiKeys[2]);
  }

  /// <summary>
  /// Cleanup API keys after tests finish
  /// </summary>
  /// <returns>A task that completes after keys are cleaned up</returns>
  public async ValueTask DisposeAsync()
  {
    if (ApiKeys is not null)
    {
      var allKeys = new[] { ApiKeys.Subscriber1.Id, ApiKeys.Subscriber2.Id, ApiKeys.Publisher.Id };
      await Task.WhenAll(allKeys.Select(DeleteApiKey));
    }
    GC.SuppressFinalize(this);
  }

  public TestApiKeys GetRequiredApiKeys()
  {
    return ApiKeys ?? throw new InvalidOperationException("API keys have not been provisioned");
  }
}