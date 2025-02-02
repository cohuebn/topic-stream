using System;
using Amazon.DynamoDBv2.DocumentModel;
using TopicStream.Functions.Connections;

namespace TopicStream.Functions.UnitTests.Connections;

public class DynamoConnectionTests
{
  [Fact]
  public void FromDynamoDocument_ConvertsIntoConnection()
  {
    var document = new Document
    {
      ["PrincipalId"] = "dj-jazzy-jeff",
      ["ConnectionId"] = "12345",
      ["ConnectedAt"] = "2021-08-01T01:02:03.4560000Z"
    };

    var result = DynamoConnectionConverter.FromDynamoDocument(document);

    Assert.Equal("dj-jazzy-jeff", result.PrincipalId);
    Assert.Equal("12345", result.ConnectionId);
    Assert.Equal(new DateTime(2021, 8, 1, 1, 2, 3, 456, DateTimeKind.Utc), result.ConnectedAt);
  }

  [Fact]
  public void ToDynamoDocument_ConvertsFromConnection()
  {
    var connection = new Connection("dj-jazzy-jeff", "12345", new DateTime(2021, 8, 1, 1, 2, 3, 456, DateTimeKind.Utc));

    var result = DynamoConnectionConverter.ToDynamoDocument(connection);

    Assert.Equal("dj-jazzy-jeff", result["PrincipalId"].AsString());
    Assert.Equal("12345", result["ConnectionId"].AsString());
    Assert.Equal("2021-08-01T01:02:03.4560000Z", result["ConnectedAt"].AsString());
  }
}