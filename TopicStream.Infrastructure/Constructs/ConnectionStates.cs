using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface IConnectionStatesProps
{
  public string? ResourcePrefix { get; }
}

internal class ConnectionStatesProps : IConnectionStatesProps
{
  public string? ResourcePrefix { get; init; }
}

/// <summary>
/// The DynamoDB table that stores the connection states for the WebSocket API.
/// </summary>
internal class ConnectionStates : Construct
{
  public ConnectionStates(Construct scope, string id, IConnectionStatesProps props) : base(scope, id)
  {
    var connectionStateTable = new Table(this, "ConnectionStates", new TableProps
    {
      TableName = ResourcePrefixer.Prefix(props.ResourcePrefix, "ConnectionStates"),
      PartitionKey = new Attribute
      {
        Name = "ConnectionId",
        Type = AttributeType.STRING,
      },
      BillingMode = BillingMode.PAY_PER_REQUEST,
    });
    connectionStateTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
    {
      IndexName = "UserIdIndex",
      PartitionKey = new Attribute
      {
        Name = "UserId",
        Type = AttributeType.STRING,
      },
      ProjectionType = ProjectionType.ALL,
    });
  }
}