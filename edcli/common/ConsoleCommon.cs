namespace edcli;

public static class ConsoleCommon
{
    private const string ENVIRONMENT_DBPATH = "EDCLI_DBPATH";
    private const string promptForReadPassword = "password";
    private const bool flagShowMaskForReadPassword = false;
    private const bool flagClearLineForReadPassword = true;
    public static string ReadPassword()
    {
        return ReadMaskinput(promptForReadPassword, flagShowMaskForReadPassword, flagClearLineForReadPassword);
    }
    public static string ReadMaskinput(string prompt, bool showMask = true, bool clearFlag = false)
    {
        string result = string.Empty;
        ConsoleKey key;
        System.Console.Write($"{prompt}: ");
        do
        {
            var keyInfo = System.Console.ReadKey(intercept: true);
            key = keyInfo.Key;
            if (key == ConsoleKey.Backspace && result.Length > 0 && showMask)
            {
                System.Console.Write("\b \b");
                result = result.Remove(result.Length - 1);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                if(showMask) System.Console.Write("*");
                result += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        if(clearFlag) ClearCurrentLine();
        return result;
    }

    public static void ClearCurrentLine()
    {
        int currentLine = System.Console.CursorTop;
        System.Console.SetCursorPosition(0, currentLine);
        System.Console.Write(new string(' ', System.Console.WindowWidth));
        System.Console.SetCursorPosition(0, currentLine);
    }

    public static string GetDatabaseFilePathEnvironmentValue()
    {
        var value = Environment.GetEnvironmentVariable(ENVIRONMENT_DBPATH);
        return value ?? "";
    }
}
