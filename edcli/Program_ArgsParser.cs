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
        var passwordOpt = new Option<string?>(new string[]{"--password","-p"}, () => {return null;} ,"Password for encryption/decryption.");
        var list = new List<Command>(){
            StashSubCommand(indexNameOpt, pathOpt, passwordOpt),
            ExtractSubCommand(indexNameOpt, passwordOpt),
        };
        return list;
    }

    public static Command StashSubCommand(Option<string> nameOpt, Option<FileInfo> pathOpt, Option<string?> passwordOpt)
    {
        var subcommand =  new Command("stash", "Stash data to Database.");
        subcommand.AddOption(nameOpt);
        subcommand.AddOption(pathOpt);
        subcommand.AddOption(passwordOpt);
        subcommand.SetHandler((name, fileInfo, password) => {
            Stash(name, fileInfo, password);
        }, nameOpt, pathOpt, passwordOpt);
        return subcommand;
    }
    public static void Stash(string indexName, FileInfo fileInfo, string? nullablePassword)
    {
        string password = nullablePassword ?? ReadPassword();
        Console.WriteLine($"Do stash with name=\"{indexName}\", file=\"{fileInfo.FullName}\" password=\"{password}\".");
    }

    public static Command ExtractSubCommand(Option<string> nameOpt, Option<string?> passwordOpt)
    {
        var subcommand =  new Command("extract", "Extract data from Database.");
        subcommand.AddOption(nameOpt);
        subcommand.AddOption(passwordOpt);
        subcommand.SetHandler((name, password) => {
            Extract(name, password);
        }, nameOpt, passwordOpt);
        return subcommand;
    }
    public static void Extract(string indexName, string? nullablePassword)
    {
        string password = nullablePassword ?? ReadPassword();
        Console.WriteLine($"Do extract with name=\"{indexName}\", password=\"{password}\".");
    }
}
