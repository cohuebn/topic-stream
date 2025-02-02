using Amazon.CDK.AWS.Lambda;

namespace TopicStream.Infrastructure.Constructs;


/// <summary>
/// A class that sets common properties for Lambda functions to avoid repetition.
/// </summary>
internal class TopicStreamFunctionProps : FunctionProps
{
  public TopicStreamFunctionProps()
  {
    Runtime = Runtime.DOTNET_8;
    // Use JSON logging by default
    LoggingFormat = Amazon.CDK.AWS.Lambda.LoggingFormat.JSON;
    ApplicationLogLevelV2 = Amazon.CDK.AWS.Lambda.ApplicationLogLevel.INFO;
    // Speeds up cold-start time compared to 256MB default, while not going too crazy with overprovisioning
    MemorySize = 512;
  }
}