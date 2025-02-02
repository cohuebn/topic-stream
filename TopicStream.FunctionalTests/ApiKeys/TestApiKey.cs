namespace TopicStream.FunctionalTests.ApiKeys;

/// <summary>
/// An API key used by tests
/// </summary>
/// <param name="Id">The unique identifier for the key</param>
/// <param name="Name">The friendly name for the API key</param>
/// <param name="ApiKey">The actual API key value to use for connection</param>
public record class TestApiKey(string Id, string Name, string ApiKey);

/// <summary>
/// An object that makes getting a particular API key easier
/// </summary>
/// <param name="Subscriber1">The API key for the first topic subscriber for our tests</param>
/// <param name="Subscriber2">The API key for the first topic subscriber for our tests</param>
/// <param name="Publisher">The API key for the topic publisher for our tests</param>
public record class TestApiKeys(TestApiKey Subscriber1, TestApiKey Subscriber2, TestApiKey Publisher);