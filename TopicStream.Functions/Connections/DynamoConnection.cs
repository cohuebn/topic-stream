using System;
using Amazon.DynamoDBv2.DocumentModel;
using TopicStream.Functions.Dynamo;

namespace TopicStream.Functions.Connections;

public static class DynamoConnectionConverter
{
  public static Connection FromDynamoDocument(Document document)
  {
    return new Connection(
      document["PrincipalId"].AsString(),
      document["ConnectionId"].AsString(),
      DynamoConverters.FromDynamoDate(document["ConnectedAt"].AsString())
    );
  }

  public static Document ToDynamoDocument(Connection connection)
  {
    return Document.FromAttributeMap(new System.Collections.Generic.Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>
    {
      ["PrincipalId"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = connection.PrincipalId },
      ["ConnectionId"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = connection.ConnectionId },
      ["ConnectedAt"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = DynamoConverters.ToDynamoDate(connection.ConnectedAt) }
    });
  }
}