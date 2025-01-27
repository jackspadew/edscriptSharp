namespace edcli;

using System.CommandLine;
using edcli.CommandLineParser;

public partial class Program
{
    public static void Main(string[] args)
    {
        var parser = new EdCliCommandLineParserFactory(new EdDataExecutor()).CreateParser();
        parser.Invoke(args);
    }
}
