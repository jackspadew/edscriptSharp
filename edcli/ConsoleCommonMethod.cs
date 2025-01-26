namespace edcli;

public partial class Program
{
    public static string ReadMaskinput(string prompt, bool showMask = true, bool clearFlag = false)
    {
        string result = string.Empty;
        ConsoleKey key;
        Console.Write($"{prompt}: ");
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;
            if (key == ConsoleKey.Backspace && result.Length > 0 && showMask)
            {
                Console.Write("\b \b");
                result = result.Remove(result.Length - 1);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                if(showMask) Console.Write("*");
                result += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        if(clearFlag) ClearCurrentLine();
        return result;
    }

    public static void ClearCurrentLine()
    {
        int currentLine = Console.CursorTop;
        Console.SetCursorPosition(0, currentLine);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLine);
    }
}
