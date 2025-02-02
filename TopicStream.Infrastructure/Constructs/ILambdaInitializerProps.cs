using Amazon.CDK.AWS.Lambda;

namespace TopicStream.Infrastructure.Constructs;

/// <summary>
/// Common properties used when initializing Lambda function(s)
/// within a construct
/// </summary>
public interface ILambdaInitializerProps
{
  public string? ResourcePrefix { get; }
  public AssetCode BundledCode { get; }
}