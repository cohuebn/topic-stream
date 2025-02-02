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
}