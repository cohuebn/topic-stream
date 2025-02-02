using System;

namespace TopicStream.Functions.Configuration;

public static class EnvironmentVariables
{
  /// <summary>
  /// Get an environment variable, throwing an exception if it is not set
  /// </summary>
  /// <param name="variableName">The name of the environment variable</param>
  /// <returns>The environment variable value when found</returns>
  /// <exception cref="InvalidOperationException">If an unknown environment variable is requested, throw an exception</exception>
  public static string GetRequired(string variableName)
  {
    var configValue = Environment.GetEnvironmentVariable(variableName);
    return configValue is null ? throw new InvalidOperationException($"{variableName} environment variable must be set to run these tests") : configValue;
  }
}
