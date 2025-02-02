using System;
using System.Security.Cryptography;
using System.Text;

namespace TopicStream.Functions.Connections;

/// This object is used to support deterministic, but secure user ids. To keep ths solution focused, I am not creating
/// a user management system. However, the only persistent, unique identifier for a user is their API key.
/// This class allows deriving a user id from an API key so that API keys aren't stored anywhere
/// outside of API Gateway.
/// The desired properties of this solution are:
/// 1. The same API key always maps to the same user id
/// 2. Different API keys always map to different user ids
/// 3. The user id can't be used to derive the user's API key

/// <summary>
/// This class provides a user id for a given connection id.
/// </summary>
public class UserIdProvider
{
  /// <summary>
  /// Get the user id for a given API key. The user id is currently
  /// just a cryptographically hashed version of the API key.
  /// </summary>
  /// <param name="apiKey">The API provided by the user</param>
  /// <returns>The user id for the given API key</returns>
  public static string GetUserId(string apiKey)
  {
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
    return Convert.ToHexString(hash);
  }
}