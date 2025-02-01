using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TopicStream.Functions;


/// <summary>
/// Object responsible for managing user connections to the application
/// </summary>
class ConnectionManager
{
  /// <summary>
  /// Register a new connection to the application
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response upon connection registration</returns>
  public static APIGatewayProxyResponse Connect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogDebug("Connection request received", request);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Connected",
    };
  }
}