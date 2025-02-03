using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;
using TopicStream.FunctionalTests.Users;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests;

/// <summary>
/// Various tests to ensure the system is working end-to-end
/// </summary>
/// <param name="apiKeyProvisioner">An object that injects API keys before tests run</param>
[Collection(nameof(ApiKeyProvisioner))]
public class SystemTests(ApiKeyProvisioner apiKeyProvisioner)
{
    private readonly TestApiKeys _testApiKeys = apiKeyProvisioner.GetRequiredApiKeys();

    [Fact]
    public async Task UnauthorizedUser_CannotConnect()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = new();

        var exception = await Assert.ThrowsAsync<WebSocketException>(
            () => user.ConnectAsync(TestConfiguration.GetUri(), testCancellation)
        );
        Assert.Contains("The server returned status code '401'", exception.Message);
    }

    [Fact]
    public async Task UserWithInvalidApiKey_CannotConnect()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = new();
        user.Options.SetRequestHeader("x-api-key", "if-i-succeed-this-test-is-broken");

        var exception = await Assert.ThrowsAsync<WebSocketException>(
            () => user.ConnectAsync(TestConfiguration.GetUri(), testCancellation)
        );
        Assert.Contains("The server returned status code '401'", exception.Message);
    }

    // This is a simple proof-of-life test to ensure the user can connect and disconnect
    // The assertion is silly, but this is a good sanity check that web socket API is allowing connections.
    // It validates:
    // 1. Auth is working as expected for a valid user
    // 2. The user only needs an API key to connect; they can disconnect without the key
    [Fact]
    public async Task AuthorizedUser_CanConnect_AndDisconnect()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber = new(_testApiKeys.Subscriber1, testCancellation);
        await subscriber.ConnectAsync();

        await subscriber.DisconnectAsync();

        Assert.True(true, "User can connect and disconnect");
    }

    [Fact]
    public async Task UserSendingMessageToUnknownAction_CanStillCloseConnection()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber = new(_testApiKeys.Subscriber1, testCancellation);
        await subscriber.ConnectAsync();

        var unknownActionMessage = new Message("what?!");
        await subscriber.SendAsync(unknownActionMessage);

        await subscriber.DisconnectAsync();

        Assert.True(true, "User can connect and disconnect even after sending unknown action");
    }

    /// <summary>
    /// Validate that:
    /// 1. Subscribers can subscribe to a topic
    /// 2. When a publisher publishes a new message, active subscribers receive it
    /// 3. Subscribers can unsubscribe from that topic
    /// </summary>
    [Fact]
    public async Task PublishedMessages_AreDeliveredToSubscriber()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber = new(_testApiKeys.Subscriber1, testCancellation);
        await subscriber.ConnectAsync();

        var topic = $"test-topic-{Guid.NewGuid()}";
        await subscriber.SubscribeAsync(topic);

        using Publisher publisher = new(_testApiKeys.Publisher, testCancellation);
        await publisher.ConnectAsync();
        await publisher.Publish(topic, "Can you see me?");

        await subscriber.WaitForMessagesAsync(1);

        var messagesForTopic = subscriber.ReceivedMessages.GetMessagesOnTopic(topic);
        Assert.Single(messagesForTopic);
        Assert.Equal("Can you see me?", messagesForTopic[0]);
    }

    /// <summary>
    /// Validate that:
    /// 1. Subscribers do not receive messages that occurred while they were not active
    /// 2. When a subscriber reconnects, they see new messages on the topic
    /// </summary>
    [Fact]
    public async Task Subscribers_SubscribedToATopic_GetNewMessagesAfterReconnection()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber1 = new(_testApiKeys.Subscriber1, testCancellation);
        await subscriber1.ConnectAsync();

        var topic = $"test-topic-{Guid.NewGuid()}";
        await subscriber1.SubscribeAsync(topic);
        await subscriber1.DisconnectAsync();

        using Subscriber subscriber2 = new(_testApiKeys.Subscriber2, testCancellation);
        await subscriber2.ConnectAsync();
        await subscriber2.SubscribeAsync(topic);

        // Missed message sent while subscriber was not active
        using Publisher publisher = new(_testApiKeys.Publisher, testCancellation);
        await publisher.ConnectAsync();
        await publisher.Publish(topic, "Where'd you go?");

        // Even though subscriber1 was not active, subscriber2 should have received the message
        // This also ensures we wait for the message to be delivered before reconnecting subscriber1
        await subscriber2.WaitForMessagesAsync(1);

        // Reconnect subscriber1 and ensure they receive the message
        await subscriber1.ConnectAsync();
        await publisher.Publish(topic, "Welcome back!");
        await subscriber1.WaitForMessagesAsync(1);

        var messagesForTopic = subscriber1.ReceivedMessages.GetMessagesOnTopic(topic);
        Assert.Single(messagesForTopic);
        Assert.Equal("Welcome back!", messagesForTopic[0]);
    }

    /// <summary>
    /// Validate that multiple subscribers can subscribe to a topic
    /// and receive the same messages from a publisher
    /// </summary>
    [Fact]
    public async Task MultipleSubscribers_SubscribedToATopic_AllGetMessages()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber1 = new(_testApiKeys.Subscriber1, testCancellation);
        await subscriber1.ConnectAsync();

        var topic = $"test-topic-{Guid.NewGuid()}";
        await subscriber1.SubscribeAsync(topic);

        using Subscriber subscriber2 = new(_testApiKeys.Subscriber2, testCancellation);
        await subscriber2.ConnectAsync();
        await subscriber2.SubscribeAsync(topic);

        using Publisher publisher = new(_testApiKeys.Publisher, testCancellation);
        await publisher.ConnectAsync();
        await publisher.Publish(topic, "#1 for all!");
        await publisher.Publish(topic, "#2 for all!");

        // Wait for both subscribers to receive the messages
        await Task.WhenAll(
            subscriber1.WaitForMessagesAsync(2),
            subscriber2.WaitForMessagesAsync(2)
        );

        var subscriber1Messages = subscriber1.ReceivedMessages.GetMessagesOnTopic(topic);
        Assert.Contains("#1 for all!", subscriber1Messages);
        Assert.Contains("#2 for all!", subscriber1Messages);

        var subscriber2Messages = subscriber2.ReceivedMessages.GetMessagesOnTopic(topic);
        Assert.Contains("#1 for all!", subscriber2Messages);
        Assert.Contains("#2 for all!", subscriber2Messages);
    }

    /// <summary>
    /// Validate that a subscriber can get messages from multiple topics
    /// at the same time
    /// </summary>
    [Fact]
    public async Task Subscriber_SubscribedToMultipleTopics_ReceivesAllMessages()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber = new(_testApiKeys.Subscriber1, testCancellation);
        await subscriber.ConnectAsync();

        var topic1 = $"test-topic-{Guid.NewGuid()}";
        await subscriber.SubscribeAsync(topic1);
        var topic2 = $"test-topic-{Guid.NewGuid()}";
        await subscriber.SubscribeAsync(topic2);

        using Publisher publisher = new(_testApiKeys.Publisher, testCancellation);
        await publisher.ConnectAsync();
        await publisher.Publish(topic1, "look here");
        await publisher.Publish(topic2, "now look over here!");

        // Wait for messages from both topics
        await subscriber.WaitForMessagesAsync(2);

        var messagesForTopic1 = subscriber.ReceivedMessages.GetMessagesOnTopic(topic1);
        Assert.Single(messagesForTopic1);
        Assert.Equal("look here", messagesForTopic1[0]);

        var messagesForTopic2 = subscriber.ReceivedMessages.GetMessagesOnTopic(topic2);
        Assert.Single(messagesForTopic2);
        Assert.Equal("now look over here!", messagesForTopic2[0]);
    }
}
