namespace edcli;

using edcli.CommandLineParser;
using LibEd.EdData;
using System.Text;

public class EdDataExecutor : IExecutor
{
    private const string ErrorDbFileDoesNotExists = "The specified database file does not exist.";
    private const string ErrorStashTargetFileDoesNotExists = "The specified stash target file does not exist.";
    private const string ErrorPathStringIsIncorrect = "The specified path string is invalid.";
# if DEBUG
    protected const bool _isDebug = true;
# else
#pragma warning disable CS0162
    protected const bool _isDebug = false;
# endif
    public static IEdDataWorker CreateEdDataWorker(string dbPath, string password)
    {
        IEdDataLogicFactory logic = new BasicEdDataLogicFactory(dbPath, password);
        IEdDataWorker worker = logic.CreateWorker();
        return worker;
    }
    public void Stash(FileInfo dbFileInfo, string indexName, FileInfo stashTargetFileInfo, string? nullablePassword)
    {
        if(!IsExists(stashTargetFileInfo.FullName, ErrorStashTargetFileDoesNotExists)) return;
        string password = nullablePassword ?? ConsoleCommon.ReadPassword();
        if(_isDebug) Console.WriteLine($"Do stash with name=\"{indexName}\", file=\"{stashTargetFileInfo.FullName}\" password=\"{password}\".");
        byte[] fileBytes = Encoding.UTF8.GetBytes(File.ReadAllText(stashTargetFileInfo.FullName));
        try
        {
            CreateEdDataWorker(dbFileInfo.FullName, password).Stash(indexName, fileBytes);
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex);
        }
    }
    public void Extract(FileInfo dbFileInfo, string indexName, string? nullablePassword)
    {
        if(!IsExists(dbFileInfo.FullName, ErrorDbFileDoesNotExists)) return;
        string password = nullablePassword ?? ConsoleCommon.ReadPassword();
        if(_isDebug) Console.WriteLine($"Do extract with name=\"{indexName}\", password=\"{password}\".");
        try
        {
            byte[] data = CreateEdDataWorker(dbFileInfo.FullName, password).Extract(indexName);
            string rawString = Encoding.UTF8.GetString(data);
            Console.WriteLine(rawString);
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex);
        }
    }
    public static bool IsExists(string path, string errorMessage)
    {
        if(File.Exists(path)) return true;
        Console.WriteLine($"{errorMessage} (\"{path}\")");
        return false;
    }
    public static bool IsCorrectPath(string path)
    {
        return true;
    }
    protected static void ShowErrorMessage(Exception ex)
    {
# if DEBUG
        throw new Exception("", ex);
# else
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(ex.Message);
        Console.ResetColor();
# endif
    }
}
