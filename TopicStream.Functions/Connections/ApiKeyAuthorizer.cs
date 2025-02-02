using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.APIGateway;
using Microsoft.Extensions.Caching.Memory;
using Amazon.APIGateway.Model;

namespace TopicStream.Functions.Connections;

/// <summary>
/// This authorizer is built as a workaround for lack of success getting API Gateway's built-in
/// "Require API Key" feature to work with WebSocket APIs. This authorizer is a simple check
/// that an API key is valid in API Gateway
/// </summary>
class ApiKeyAuthorizer
{
  private Task? _inProgressCacheRefresh;
  private readonly IMemoryCache _apiKeyCache;

  public ApiKeyAuthorizer()
  {
    _inProgressCacheRefresh = null;
    // API Gateway has a hard limit of 10000 keys per request, so we don't need to worry about
    // restricting cache size with this implementation
    _apiKeyCache = new MemoryCache(new MemoryCacheOptions());
  }

  /// <summary>
  /// Refresh the cache from API Gateway to ensure we have the latest keys
  /// </summary>
  /// <returns>A task that completes once the cache has refreshed</returns>
  private async Task RefreshCache()
  {
    // Looping to handle hard limit of 500 keys per request and paginated API to get keys
    string? position = null;
    var hasMoreData = true;
    var pageSize = 500;
    while (hasMoreData)
    {
      var response = await new AmazonAPIGatewayClient().GetApiKeysAsync(new GetApiKeysRequest
      {
        Limit = pageSize,
        Position = position,
        IncludeValues = true
      });
      // Cache the keys for 10 minutes; this means when access is revoked,
      // it will take up to 10 minutes for the key to stop working. In a real production solution
      // I would use a better method of getting closer-to-real-time API keys
      response.Items.ForEach(apiKey => _apiKeyCache.Set(apiKey.Value, true, TimeSpan.FromMinutes(10)));

      position = response.Position;
      hasMoreData = response.Items.Count >= pageSize;
    }
  }

  /// <summary>
  /// Trigger a cache refresh and ensure only one refresh is in progress at a time
  /// across multiple requests
  /// </summary>
  /// <returns>A task that completes when the cache is refreshed</returns>
  private async Task TriggerSharedCacheRefresh()
  {
    try
    {
      _inProgressCacheRefresh = RefreshCache();
      await _inProgressCacheRefresh;
    }
    finally
    {
      _inProgressCacheRefresh = null;
    }
  }

  public async Task<APIGatewayCustomAuthorizerResponse> Authorize(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
  {
    request.Headers.TryGetValue("x-api-key", out var apiKey);
    if (apiKey is null)
    {
      context.Logger.LogWarning("Request made without an API key");
      throw new Exception("Unauthorized");
    }

    var isApiKeyInCache = _apiKeyCache.TryGetValue(apiKey, out bool _);
    if (isApiKeyInCache)
    {
      return new APIGatewayCustomAuthorizerResponse
      {
        PrincipalID = PrincipalIdProvider.GetPrincipalId(apiKey),
        PolicyDocument = IamPolicyGenerator.GenerateAllowPolicy(request.MethodArn)
      };
    }

    // Refresh the cache to make sure this isn't a new key
    if (_inProgressCacheRefresh is null)
    {
      await TriggerSharedCacheRefresh();
    }

    var isApiKeyInRefreshedCache = _apiKeyCache.TryGetValue(apiKey, out bool _);
    if (!isApiKeyInRefreshedCache)
    {
      context.Logger.LogWarning("Request made with an unknown API key");
      throw new Exception("Unauthorized");
    }

    return new APIGatewayCustomAuthorizerResponse
    {
      PrincipalID = PrincipalIdProvider.GetPrincipalId(apiKey),
      PolicyDocument = IamPolicyGenerator.GenerateAllowPolicy(request.MethodArn)
    };
  }
}