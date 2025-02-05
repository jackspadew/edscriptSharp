using System;

namespace PsEdScript.Parser;

public class ShebangParser
{
    public bool IsShebang = false;
    public string Command;
    public string CommandExact;
    public static ShebangParser Parse(string text)
    {
        ShebangParser result = new ShebangParser(false, "", "");
        return result;
    }

    protected ShebangParser(bool isShebang, string command, string commandExact)
    {
        this.IsShebang = isShebang;
        this.Command = command;
        this.CommandExact = commandExact;
    }
}
