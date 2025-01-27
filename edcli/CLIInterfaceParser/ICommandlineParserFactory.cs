namespace edcli.CommandLineParser;

using System.CommandLine;

public interface ICommandlineParserFactory
{
    Command CreateParser();
}
