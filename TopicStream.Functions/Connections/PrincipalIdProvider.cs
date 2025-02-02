using System;
using System.Security.Cryptography;
using System.Text;

namespace TopicStream.Functions.Connections;

/// This object is used to support deterministic, but secure principal ids (a.k.a. user ids). To keep ths solution focused,
/// I am not creating a user management system. However, the only persistent, unique identifier for a principal is their API key.
/// This class allows deriving a principal id from an API key so that API keys aren't stored anywhere
/// outside of API Gateway.
/// The desired properties of this solution are:
/// 1. The same API key always maps to the same principal id
/// 2. Different API keys always map to different principal ids
/// 3. The principal id can't be used to derive the principal's API key

/// <summary>
/// This class provides a principal id for a given connection id.
/// </summary>
public class PrincipalIdProvider
{
  /// <summary>
  /// Get the principal id for a given API key. The principal id is currently
  /// just a cryptographically hashed version of the API key.
  /// </summary>
  /// <param name="apiKey">The API provided by the principal</param>
  /// <returns>The principal id for the given API key</returns>
  public static string GetPrincipalId(string apiKey)
  {
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
    return Convert.ToHexString(hash);
  }
}