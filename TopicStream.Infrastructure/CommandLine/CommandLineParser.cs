using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace TopicStream.Infrastructure.CommandLine;

internal static class CommandLineParser
{
  private static ParsingStatus GetRequestTypeFromErrors(IEnumerable<Error> errors)
  {
    return errors.Any(error => error is HelpRequestedError || error is VersionRequestedError) ?
      ParsingStatus.InformationOnlyRequest :
      ParsingStatus.Halt;
  }

  public static CommandLineParsingResult Parse(string[] args)
  {
    var parsingResult = Parser.Default.ParseArguments<CommandLineOptions?>(args);
    return parsingResult.Errors.Any() ?
      new CommandLineParsingResult(GetRequestTypeFromErrors(parsingResult.Errors), null) :
      new CommandLineParsingResult(ParsingStatus.Proceed, parsingResult.Value);
  }
}