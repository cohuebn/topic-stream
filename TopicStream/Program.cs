using Amazon.CDK;


namespace TopicStream;

/// <summary>
/// This is the entry point of the CDK application. It's responsible for creating the TopicStream stack
/// and creating the CloudFormation template to deploy into AWS
/// </summary>
sealed class Program
{
    public static void Main()
    {
        var app = new App();
        _ = new TopicStreamStack(app, "TopicStreamStack", new StackProps());
        app.Synth();
    }
}
