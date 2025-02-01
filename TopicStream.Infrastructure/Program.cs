using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using TopicStream.Infrastructure.Constructs;
using TopicStream.Infrastructure.CommandLine;

// Get command line arguments and stop execution if the parser decides not to proceed (--help, --version, bad arguments, etc.)
var cliParsingResult = CommandLineParser.Parse(args);
var cliOptions = cliParsingResult.Options;
if (cliParsingResult.ParsingStatus != ParsingStatus.Proceed || cliOptions is null)
{
  var exitCode = cliParsingResult.ParsingStatus == ParsingStatus.InformationOnlyRequest ? 0 : 1;
  System.Environment.Exit(exitCode);
}

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
var bundledCode = Code.FromAsset(cliOptions.FunctionsProject, new Amazon.CDK.AWS.S3.Assets.AssetOptions
{
  Bundling = buildOptions
});

// Create the AWS resources for the stack
var app = new App();

_ = new AccountStack(app, "AccountStack");

_ = new TopicStreamStack(app, cliOptions.StackId, new TopicStreamStackProps
{
  BundledCode = bundledCode
});


app.Synth();