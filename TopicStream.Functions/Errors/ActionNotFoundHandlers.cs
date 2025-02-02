using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TopicStream.Functions.Connections;
using TopicStream.Functions.Messages;

namespace TopicStream.Functions.Errors;


/// <summary>
/// Object responsible for managing calls to unknown routes/actions
/// </summary>
class ActionNotFoundHandlers
{
  /// <summary>
  /// Log the call to the unknown action and return a 404 response
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the request</returns>
  public static APIGatewayProxyResponse HandleUnknownAction(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var connection = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);
    var message = JsonSerializer.Deserialize<Message>(request.Body, MessageSerializerOptions.Standard);
    var action = message?.Action ?? "No action";
    context.Logger.LogWarning("Unknown action '{action}': Connection {@connection}", action, connection);
    return new APIGatewayProxyResponse
    {
      StatusCode = 404,
      Body = "Unknown action",
    };
  }
}