using System.Collections.Generic;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ISubscriptionFunctionsProps : ILambdaInitializerProps
{
  public Table SubscriptionsTable { get; }
}

internal class SubscriptionFunctionsProps : ISubscriptionFunctionsProps
{
  public string? ResourcePrefix { get; init; }
  public required AssetCode BundledCode { get; init; }
  public required Table SubscriptionsTable { get; init; }
}

/// <summary>
/// The Lambda functions that handle user subscriptions to topics.
/// </summary>
internal class SubscriptionFunctions : Construct
{
  public Function SubscribeFunction { get; }
  public Function UnsubscribeFunction { get; }

  public SubscriptionFunctions(Construct scope, string id, ISubscriptionFunctionsProps props) : base(scope, id)
  {
    SubscribeFunction = new Function(this, "Subscribe", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Subscribe"),
      Handler = "TopicStream.Functions::TopicStream.Functions.Subscriptions.SubscriptionHandlers::Subscribe",
      Code = props.BundledCode,
      Environment = new Dictionary<string, string>
      {
        ["SUBSCRIPTIONS_TABLE_NAME"] = props.SubscriptionsTable.TableName,
      },
    });
    props.SubscriptionsTable.GrantReadWriteData(SubscribeFunction);


    UnsubscribeFunction = new Function(this, "Unsubscribe", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Unsubscribe"),
      Handler = "TopicStream.Functions::TopicStream.Functions.Subscriptions.SubscriptionHandlers::Unsubscribe",
      Code = props.BundledCode,
      Environment = new Dictionary<string, string>
      {
        ["SUBSCRIPTIONS_TABLE_NAME"] = props.SubscriptionsTable.TableName,
      },
    });
    props.SubscriptionsTable.GrantReadWriteData(UnsubscribeFunction);

  }
}