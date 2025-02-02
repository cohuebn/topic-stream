using Amazon.DynamoDBv2.DocumentModel;
using TopicStream.Functions.Dynamo;

namespace TopicStream.Functions.Subscriptions;

public static class DynamoSubscriptionConverter
{
  public static Subscription FromDynamoDocument(Document document)
  {
    return new Subscription(
      document["Topic"].AsString(),
      document["PrincipalId"].AsString(),
      DynamoConverters.FromDynamoDate(document["SubscribedAt"].AsString())
    );
  }

  public static Document ToDynamoDocument(Subscription subscription)
  {
    return Document.FromAttributeMap(new System.Collections.Generic.Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>
    {
      ["Topic"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = subscription.Topic },
      ["PrincipalId"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = subscription.PrincipalId },
      ["SubscribedAt"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = DynamoConverters.ToDynamoDate(subscription.SubscribedAt) }
    });
  }
}