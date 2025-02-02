using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface IConnectionFunctionsProps
{
  public string? ResourcePrefix { get; }
  public AssetCode BundledCode { get; }
}

internal class ConnectionFunctionsProps : IConnectionFunctionsProps
{
  public string? ResourcePrefix { get; init; }
  public required AssetCode BundledCode { get; init; }
}

/// <summary>
/// The Lambda functions that handle WebSocket connections and disconnections.
/// </summary>
internal class ConnectionFunctions : Construct
{
  public Function ConnectFunction { get; }
  public Function DisconnectFunction { get; }

  public ConnectionFunctions(Construct scope, string id, IConnectionFunctionsProps props) : base(scope, id)
  {
    ConnectFunction = new Function(this, "Connect", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Connect"),
      Handler = "TopicStream.Functions::TopicStream.Functions.Connections.Connection::Connect",
      Code = props.BundledCode,
    });

    DisconnectFunction = new Function(this, "Disconnect", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Disconnect"),
      Handler = "TopicStream.Functions::TopicStream.Functions.Connections.Connection::Connect",
      Code = props.BundledCode,
    });
  }
}