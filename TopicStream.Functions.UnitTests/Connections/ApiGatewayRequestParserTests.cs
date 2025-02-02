using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using TopicStream.Functions.Connections;

namespace TopicStream.Functions.UnitTests.Connections;

public class ApiGatewayRequestParserTests
{
  [Fact]
  public void GetRequiredPrincipalIdFromRequest_ExtractsAuthorizedRequest()
  {
    var expectedPrincipalId = "dj-jazzy-jeff";
    var authorizerContext = new APIGatewayCustomAuthorizerContext
    {
      ["principalId"] = "dj-jazzy-jeff"
    };
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = authorizerContext
      }
    };

    var result = ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request);

    Assert.Equal(expectedPrincipalId, result);
  }

  [Fact]
  public void GetRequiredPrincipalIdFromRequest_ThrowsWhenNoPrincipalId()
  {
    var authorizerContext = new APIGatewayCustomAuthorizerContext
    {
      ["someOtherKey"] = "someOtherValue"
    };
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = authorizerContext
      }
    };

    var exception = Assert.Throws<KeyNotFoundException>(() => ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request));
    Assert.Equal("Principal ID not found in the request", exception.Message);
  }

  [Fact]
  public void GetRequiredPrincipalIdFromRequest_ThrowsWhenNoAuthorizer()
  {
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = null
      }
    };

    var exception = Assert.Throws<KeyNotFoundException>(() => ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request));
    Assert.Equal("Authorizer not found in the request", exception.Message);
  }

  [Fact]
  public void GetRequiredConnectionIdFromRequest_ExtractsFromWebSocketRequest()
  {
    var expectedConnectionId = "im-a-connection-id";
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        ConnectionId = expectedConnectionId
      }
    };

    var result = ApiGatewayRequestParser.GetRequiredConnectionIdFromRequest(request);

    Assert.Equal(expectedConnectionId, result);
  }

  [Fact]
  public void GetRequiredConnectionIdFromRequest_ThrowsWhenNoConnectionId()
  {
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext()
    };

    var exception = Assert.Throws<KeyNotFoundException>(() => ApiGatewayRequestParser.GetRequiredConnectionIdFromRequest(request));
    Assert.Equal("Connection ID not found in the request", exception.Message);
  }

  [Fact]
  public void GetRequiredConnectionAtFromRequest_ExtractsFromWebSocketRequest()
  {
    var connectedAtEpochMillis = 1738511121229;
    var expectedConnectedAt = DateTime.Parse("2025-02-02T15:45:21.229Z").ToUniversalTime();
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        ConnectedAt = connectedAtEpochMillis
      }
    };

    var result = ApiGatewayRequestParser.GetConnectedAtFromRequest(request);

    Assert.Equal(expectedConnectedAt, result);
  }

  [Fact]
  public void GetRequiredConnectionAtFromRequest_ThrowsWhenNoConnectionAt()
  {
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext()
    };

    var exception = Assert.Throws<KeyNotFoundException>(() => ApiGatewayRequestParser.GetRequiredConnectionIdFromRequest(request));
    Assert.Equal("Connection ID not found in the request", exception.Message);
  }

  [Fact]
  public void GetAuthorizedWebSocketRequest_ExtractsAllRequestDetails()
  {
    var expectedPrincipalId = "dj-jazzy-jeff";
    var authorizerContext = new APIGatewayCustomAuthorizerContext
    {
      ["principalId"] = "dj-jazzy-jeff"
    };
    var expectedConnectionId = "im-a-connection-id";
    var connectedAtEpochMillis = 1738511121229;
    var expectedConnectedAt = DateTime.Parse("2025-02-02T15:45:21.229Z").ToUniversalTime();
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = authorizerContext,
        ConnectionId = expectedConnectionId,
        ConnectedAt = connectedAtEpochMillis,
      }
    };

    var result = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);

    Assert.Equal(expectedPrincipalId, result.PrincipalId);
    Assert.Equal(expectedConnectionId, result.ConnectionId);
    Assert.Equal(expectedConnectedAt, result.ConnectedAt);
  }
}