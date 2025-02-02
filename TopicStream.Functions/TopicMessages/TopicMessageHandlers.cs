using System.Text.Json;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TopicStream.Functions.Connections;
using TopicStream.Functions.Messages;
using Amazon.SQS.Model;
using TopicStream.Functions.Configuration;

namespace TopicStream.Functions.TopicMessages;


/// <summary>
/// Object responsible for managing new messages meant to be
/// sent to topics within the application
/// </summary>
class TopicMessageHandlers
{
  private readonly AmazonSQSClient _sqsClient;

  public TopicMessageHandlers()
  {
    _sqsClient = new AmazonSQSClient();
  }

  /// <summary>
  /// Publish the provided message to the requested topic
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the publish request</returns>
  public async Task<APIGatewayProxyResponse> Publish(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var connection = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);
    if (!RequestMessageParser.TryGetMessage<PublishMessage>(request, context, out var publishMessage) || publishMessage is null)
    {
      return new APIGatewayProxyResponse { StatusCode = 400, Body = "Invalid subscription message" };
    }
    context.Logger.LogDebug(
      "Publish request received: Topic {topic}, Message {topicMessage}, Principal {principal}",
      publishMessage.Topic,
      publishMessage.Message,
      connection.PrincipalId
    );

    await _sqsClient.SendMessageAsync(new SendMessageRequest
    {
      QueueUrl = TopicMessagesConfiguration.QueueUrl,
      MessageBody = JsonSerializer.Serialize(publishMessage, MessageSerializerOptions.Standard),
    });
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Published message",
    };
  }
}