using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ISubscriptionsProps
{
  public string? ResourcePrefix { get; }
}

internal class SubscriptionProps : ISubscriptionsProps
{
  public string? ResourcePrefix { get; init; }
}

/// <summary>
/// The DynamoDB table that stores the connection states for the WebSocket API.
/// </summary>
internal class Subscriptions : Construct
{
  public Table SubscriptionsTable { get; init; }

  public Subscriptions(Construct scope, string id, ISubscriptionsProps props) : base(scope, id)
  {
    var topicAttribute = new Attribute
    {
      Name = "Topic",
      Type = AttributeType.STRING,
    };
    var principalIdAttribute = new Attribute
    {
      Name = "PrincipalId",
      Type = AttributeType.STRING,
    };
    // Primary index supports fast lookup by topic
    SubscriptionsTable = new Table(this, "Subscriptions", new TableProps
    {
      TableName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Subscriptions"),
      RemovalPolicy = Amazon.CDK.RemovalPolicy.DESTROY,
      PartitionKey = topicAttribute,
      SortKey = principalIdAttribute,
      BillingMode = BillingMode.PAY_PER_REQUEST,
    });

    // Secondary index supports fast lookup by principal id
    SubscriptionsTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
    {
      IndexName = "PrincipalIdIndex",
      PartitionKey = principalIdAttribute,
      SortKey = topicAttribute,
      ProjectionType = ProjectionType.ALL,
    });
  }
}