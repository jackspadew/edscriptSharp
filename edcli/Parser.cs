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
        return new List<Command>(){
            StashSubCommand(),
            ExtractSubCommand(),
        };
    }

    public static Command StashSubCommand()
    {
        var subcommand =  new Command("stash", "Stash data to Database.");
        subcommand.SetHandler(() => {
            Stash();
        });
        return subcommand;
    }
    public static void Stash()
    {
        Console.WriteLine($"Do stash");
    }

    public static Command ExtractSubCommand()
    {
        var subcommand =  new Command("extract", "Extract data from Database.");
        subcommand.SetHandler(() => {
            Extract();
        });
        return subcommand;
    }
    public static void Extract()
    {
        Console.WriteLine($"Do extract");
    }
}
