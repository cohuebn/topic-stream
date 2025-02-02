using System;
using Amazon.DynamoDBv2.DocumentModel;

namespace TopicStream.Functions.Connections;

public static class DynamoConnectionConverter
{
  public static string ToConnectedAtString(DateTime connectedAt)
  {
    return connectedAt.ToString("o");
  }

  public static Connection FromDynamoDocument(Document document)
  {
    return new Connection(
      document["PrincipalId"].AsString(),
      document["ConnectionId"].AsString(),
      DateTime.Parse(document["ConnectedAt"].AsString()).ToUniversalTime()
    );
  }

  public static Document ToDynamoDocument(Connection connection)
  {
    return Document.FromAttributeMap(new System.Collections.Generic.Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>
    {
      ["PrincipalId"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = connection.PrincipalId },
      ["ConnectionId"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = connection.ConnectionId },
      ["ConnectedAt"] = new Amazon.DynamoDBv2.Model.AttributeValue { S = ToConnectedAtString(connection.ConnectedAt) }
    });
  }
}