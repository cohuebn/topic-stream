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
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = new APIGatewayCustomAuthorizerContext
        {
          Claims = new Dictionary<string, string>
          {
            ["principalId"] = expectedPrincipalId
          }
        }
      }
    };

    var result = ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request);

    Assert.Equal(expectedPrincipalId, result);
  }

  [Fact]
  public void GetRequiredPrincipalIdFromRequest_ThrowsWhenNoPrincipalId()
  {
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = new APIGatewayCustomAuthorizerContext
        {
          Claims = []
        }
      }
    };

    var exception = Assert.Throws<KeyNotFoundException>(() => ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request));
    Assert.Equal("Principal ID not found in the request", exception.Message);
  }

  [Fact]
  public void GetRequiredPrincipalIdFromRequest_ThrowsWhenNoClaims()
  {
    var request = new APIGatewayProxyRequest
    {
      RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
      {
        Authorizer = new APIGatewayCustomAuthorizerContext
        {
          Claims = null
        }
      }
    };

    var exception = Assert.Throws<KeyNotFoundException>(() => ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request));
    Assert.Equal("Principal ID not found in the request", exception.Message);
  }
}