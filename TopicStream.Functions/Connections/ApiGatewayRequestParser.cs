using System;
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

  public static string GetRequiredConnectionIdFromRequest(APIGatewayProxyRequest request)
  {
    var connectionId = request.RequestContext?.ConnectionId;
    return connectionId ?? throw new KeyNotFoundException("Connection ID not found in the request");
  }

  public static DateTime GetConnectedAtFromRequest(APIGatewayProxyRequest request)
  {
    var connectedAt = request.RequestContext?.ConnectedAt;
    if (connectedAt is null)
    {
      throw new KeyNotFoundException("'Connection At' value not found in the request");
    }
    return DateTimeOffset.FromUnixTimeMilliseconds(connectedAt.Value).UtcDateTime;
  }
}