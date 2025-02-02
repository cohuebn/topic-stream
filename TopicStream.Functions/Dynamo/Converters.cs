using System;

namespace TopicStream.Functions.Dynamo;

/// <summary>
/// Common converters for getting data to/from DynamoDB
/// </summary>
public static class DynamoConverters
{
  /// <summary>
  /// Convert a DateTime into a string that can be stored in DynamoDB
  /// </summary>
  /// <param name="date">The date/time to convert</param>
  /// <returns>The stringified date in the "round-trip date/time" format</returns>
  public static string ToDynamoDate(DateTime date)
  {
    return date.ToString("o");
  }

  /// <summary>
  /// Convert a Dynamo string representing a date/time into an actual DateTime object
  /// </summary>
  /// <param name="dynamoDate">The stringified date/time from Dynamo</param>
  /// <returns>The parsed DateTime</returns>
  public static DateTime FromDynamoDate(string dynamoDate)
  {
    return DateTime.Parse(dynamoDate).ToUniversalTime();
  }
}