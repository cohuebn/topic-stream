using System.Collections.Generic;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface IConnectionFunctionsProps
{
  public string? ResourcePrefix { get; }
  public AssetCode BundledCode { get; }
  public Table ConnectionStatesTable { get; }
}

internal class ConnectionFunctionsProps : IConnectionFunctionsProps
{
  public string? ResourcePrefix { get; init; }
  public required AssetCode BundledCode { get; init; }
  public required Table ConnectionStatesTable { get; init; }
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
      Handler = "TopicStream.Functions::TopicStream.Functions.Connections.ConnectionHandlers::Connect",
      Code = props.BundledCode,
      Environment = new Dictionary<string, string>
      {
        ["CONNECTION_STATES_TABLE_NAME"] = props.ConnectionStatesTable.TableName,
      },
    });
    props.ConnectionStatesTable.GrantReadWriteData(ConnectFunction);

    DisconnectFunction = new Function(this, "Disconnect", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Disconnect"),
      Handler = "TopicStream.Functions::TopicStream.Functions.Connections.ConnectionHandlers::Disconnect",
      Code = props.BundledCode,
      Environment = new Dictionary<string, string>
      {
        ["CONNECTION_STATES_TABLE_NAME"] = props.ConnectionStatesTable.TableName,
      },
    });
    props.ConnectionStatesTable.GrantReadWriteData(DisconnectFunction);
  }
}