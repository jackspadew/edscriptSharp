using System;

namespace PsEdScript.Parser;

public class ShebangParser
{
    protected const string SHEBANG_PREFIX = "#!";
    protected static readonly char[] SPLIT_DELIMITER_WHITESPACES = new char[] { ' ', '\t' };
    protected static readonly char[] SPLIT_DELIMITER_PATH_SEPARATOR = new char[] { '/', '\\' };
    public bool IsShebang = false;
    public string Command;
    public string CommandExact;
    public static ShebangParser Parse(string text)
    {
        if (!text.StartsWith(SHEBANG_PREFIX))
        {
            return new ShebangParser(false, "", "");
        }
        string firstLine = text.Split('\n')[0];
        string trimedCommand = firstLine.Substring(SHEBANG_PREFIX.Length)
            .TrimStart()
            .TrimEnd();
        string[] trimedWords = trimedCommand.Split(SPLIT_DELIMITER_WHITESPACES, StringSplitOptions.RemoveEmptyEntries);
        if(trimedWords.Length == 2)
        {
            return new ShebangParser(true, trimedCommand, trimedWords[1]);
        }
        else if(trimedWords.Length == 1)
        {
            string[] pathItems = trimedCommand.Split(SPLIT_DELIMITER_PATH_SEPARATOR);
            return new ShebangParser(true, trimedCommand, pathItems[pathItems.Length-1]);
        }
        return new ShebangParser(false, "", "");
    }

    protected ShebangParser(bool isShebang, string command, string commandExact)
    {
        this.IsShebang = isShebang;
        this.Command = command;
        this.CommandExact = commandExact;
    }
}
