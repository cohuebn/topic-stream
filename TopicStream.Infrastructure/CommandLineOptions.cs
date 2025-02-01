namespace TopicStream.Infrastructure;

using CommandLine;

/// <summary>
/// Command line options for the deployment stack
/// </summary>
public class CommandLineOptions
{
  /// <summary>
  /// The path to the project that holds Lambda function code; this allows running from any working directory
  /// without assuming the relative path to the Lambda functions project. The default assumes we're running
  /// from within the TopicStream.Infrastructure project directory.
  /// </summary>
  [Option("functions-project", Required = false, HelpText = "The path to the project that holds Lambda function code.", Default = "../TopicStream.Functions")]
  public string FunctionsProject { get; set; }
}