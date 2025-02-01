using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using CommandLine;
using TopicStream.Infrastructure;

// Get CLI arguments
var options = Parser.Default.ParseArguments<CommandLineOptions>(args).Value;

// Bundle up app code optimized for Lambda runtime
var buildOptions = new BundlingOptions
{
  Image = Runtime.DOTNET_8.BundlingImage,
  User = "root",
  OutputType = BundlingOutput.NOT_ARCHIVED,
  Command = [
        "/bin/sh",
          "-c",
          " dotnet tool install -g Amazon.Lambda.Tools" +
          " && dotnet build" +
          " && dotnet publish -c Release --self-contained false -o /asset-output"
      ]
};
var bundledCode = Code.FromAsset(options.FunctionsProject, new Amazon.CDK.AWS.S3.Assets.AssetOptions
{
  Bundling = buildOptions
});

// Create the AWS resources for the stack
var app = new App();
_ = new TopicStreamStack(app, "TopicStreamStack", bundledCode, new StackProps());
app.Synth();