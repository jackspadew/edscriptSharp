namespace edcli;

using System.CommandLine;

public partial class Program
{
    public static void RunByCommandParser(string[] args)
    {
        var rootCommand = CommandParser();
        rootCommand.Invoke(args);
    }
    public static RootCommand CommandParser()
    {
        RootCommand rootCommand = new("edcli");
        foreach(var subcommand in SubcommandList())
        {
            rootCommand.Add(subcommand);
        }
        return rootCommand;
    }

    public static List<Command> SubcommandList()
    {
        var indexNameOpt = new Option<string>(new string[]{"--name","-n"}, "Index name of the stashed data."){IsRequired = true};
        var pathOpt = new Option<FileInfo>(new string[]{"--file","-f"}, "File path for stashing."){IsRequired = true};
        var dbPathOpt = new Option<FileInfo>(new string[]{"--db"}, () => {return new FileInfo(GetDatabaseFilePathEnvironmentValue());}, "Database file path.");
        var passwordOpt = new Option<string?>(new string[]{"--password","-p"}, () => {return null;} ,"Password for encryption/decryption.");
        var list = new List<Command>(){
            StashSubCommand(dbPathOpt, indexNameOpt, pathOpt, passwordOpt),
            ExtractSubCommand(dbPathOpt, indexNameOpt, passwordOpt),
        };
        return list;
    }

    public static Command StashSubCommand(Option<FileInfo> dbPathOpt, Option<string> nameOpt, Option<FileInfo> pathOpt, Option<string?> passwordOpt)
    {
        var subcommand =  new Command("stash", "Stash data to Database.");
        subcommand.AddOption(dbPathOpt);
        subcommand.AddOption(nameOpt);
        subcommand.AddOption(pathOpt);
        subcommand.AddOption(passwordOpt);
        subcommand.SetHandler((dbFileInfo, name, fileInfo, password) => {
            Stash(dbFileInfo, name, fileInfo, password);
        }, dbPathOpt, nameOpt, pathOpt, passwordOpt);
        return subcommand;
    }
    public static Command ExtractSubCommand(Option<FileInfo> dbPathOpt, Option<string> nameOpt, Option<string?> passwordOpt)
    {
        var subcommand =  new Command("extract", "Extract data from Database.");
        subcommand.AddOption(dbPathOpt);
        subcommand.AddOption(nameOpt);
        subcommand.AddOption(passwordOpt);
        subcommand.SetHandler((dbFileInfo, name, password) => {
            Extract(dbFileInfo, name, password);
        }, dbPathOpt, nameOpt, passwordOpt);
        return subcommand;
    }
}
