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
    var claims = request.RequestContext?.Authorizer?.Claims;

    if (claims is null || !claims.TryGetValue("principalId", out string? principalId) || principalId is null)
    {
      throw new KeyNotFoundException("Principal ID not found in the request");
    }

    return principalId;
  }
}