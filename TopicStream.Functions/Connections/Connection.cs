using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace TopicStream.Functions.Connections;


/// <summary>
/// Object responsible for managing user connections to the application
/// </summary>
class Connection
{
  /// <summary>
  /// Register a user connecting to the application
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the connection request</returns>
  public static APIGatewayProxyResponse Connect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var principal = ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request);
    context.Logger.LogDebug("Connection request received {principal}", principal);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Connected",
    };
  }

  /// <summary>
  /// Register a user disconnecting from the application
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the disconnection request</returns>
  public static APIGatewayProxyResponse Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var principal = ApiGatewayRequestParser.GetRequiredPrincipalIdFromRequest(request);
    context.Logger.LogDebug("Disconnection request received {principal}", principal);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Disconnected",
    };
  }
}