namespace edcli.CommandLineParser;

using System.CommandLine;

public class EdCliCommandLineParserFactory : ICommandlineParserFactory
{
    private IExecutor _executer;
    public EdCliCommandLineParserFactory(IExecutor executer)
    {
        _executer = executer;
    }

    public Command CreateParser()
    {
        RootCommand rootCommand = new("edcli");
        foreach(var subcommand in SubcommandList())
        {
            rootCommand.Add(subcommand);
        }
        return rootCommand;
    }

    public List<Command> SubcommandList()
    {
        var indexNameOpt = new Option<string>(new string[]{"--name","-n"}, "Index name of the stashed data."){IsRequired = true};
        var pathOpt = new Option<FileInfo>(new string[]{"--file","-f"}, "File path for stashing."){IsRequired = true};
        var dbPathOpt = new Option<FileInfo>(new string[]{"--db"}, "Database file path.");
        var passwordOpt = new Option<string?>(new string[]{"--password","-p"}, () => {return null;} ,"Password for encryption/decryption.");
        var list = new List<Command>(){
            StashSubCommand(dbPathOpt, indexNameOpt, pathOpt, passwordOpt),
            ExtractSubCommand(dbPathOpt, indexNameOpt, passwordOpt),
        };
        return list;
    }

    public Command StashSubCommand(Option<FileInfo> dbPathOpt, Option<string> nameOpt, Option<FileInfo> pathOpt, Option<string?> passwordOpt)
    {
        var subcommand =  new Command("stash", "Stash data to Database.");
        subcommand.AddOption(dbPathOpt);
        subcommand.AddOption(nameOpt);
        subcommand.AddOption(pathOpt);
        subcommand.AddOption(passwordOpt);
        subcommand.SetHandler((dbFileInfo, name, fileInfo, password) => {
            _executer.Stash(dbFileInfo, name, fileInfo, password);
        }, dbPathOpt, nameOpt, pathOpt, passwordOpt);
        return subcommand;
    }
    public Command ExtractSubCommand(Option<FileInfo> dbPathOpt, Option<string> nameOpt, Option<string?> passwordOpt)
    {
        var subcommand =  new Command("extract", "Extract data from Database.");
        subcommand.AddOption(dbPathOpt);
        subcommand.AddOption(nameOpt);
        subcommand.AddOption(passwordOpt);
        subcommand.SetHandler((dbFileInfo, name, password) => {
            _executer.Extract(dbFileInfo, name, password);
        }, dbPathOpt, nameOpt, passwordOpt);
        return subcommand;
    }
}
