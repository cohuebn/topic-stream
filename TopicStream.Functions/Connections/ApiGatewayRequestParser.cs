using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;

namespace TopicStream.Functions.Connections;

/// <summary>
/// A helper class for parsing harder-to-reach details of API Gateway requests
/// </summary>
public static class ApiGatewayRequestParser
{
  public static string GetRequiredPrincipalIdFromRequest(APIGatewayProxyRequest request)
  {
    var authorizer = request.RequestContext?.Authorizer;
    if (authorizer is null)
    {
      throw new KeyNotFoundException("Authorizer not found in the request");
    }

    if (!authorizer.TryGetValue("principalId", out var principalId) || principalId is null)
    {
      throw new KeyNotFoundException("Principal ID not found in the request");
    }
    return $"{principalId}";
  }
}