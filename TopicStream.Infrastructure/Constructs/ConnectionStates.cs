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
  public Table ConnectionStatesTable { get; init; }

  public ConnectionStates(Construct scope, string id, IConnectionStatesProps props) : base(scope, id)
  {
    // A timestamp attribute to support sorting by connection creation time
    var connectedAt = new Attribute
    {
      Name = "ConnectedAt",
      Type = AttributeType.STRING,
    };

    // Primary index supports fast lookup by connection id
    ConnectionStatesTable = new Table(this, "ConnectionStates", new TableProps
    {
      TableName = ResourcePrefixer.Prefix(props.ResourcePrefix, "ConnectionStates"),
      RemovalPolicy = Amazon.CDK.RemovalPolicy.DESTROY,
      PartitionKey = new Attribute
      {
        Name = "ConnectionId",
        Type = AttributeType.STRING,
      },
      SortKey = connectedAt,
      BillingMode = BillingMode.PAY_PER_REQUEST,
    });

    // Secondary index supports fast lookup by user id
    ConnectionStatesTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
    {
      IndexName = "PrincipalIdIndex",
      PartitionKey = new Attribute
      {
        Name = "PrincipalId",
        Type = AttributeType.STRING,
      },
      SortKey = connectedAt,
      ProjectionType = ProjectionType.ALL,
    });
  }
}