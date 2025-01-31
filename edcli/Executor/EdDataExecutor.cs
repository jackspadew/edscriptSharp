namespace edcli;

using edcli.CommandLineParser;
using LibEd.EdData;
using System.Text;

public class EdDataExecutor : IExecutor
{
    private const string ErrorDbFileDoesNotExists = "The specified database file does not exist.";
    private const string ErrorStashTargetFileDoesNotExists = "The specified stash target file does not exist.";
    private const string ErrorPathStringIsIncorrect = "The specified path string is invalid.";
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
        Console.WriteLine($"Do stash with name=\"{indexName}\", file=\"{stashTargetFileInfo.FullName}\" password=\"{password}\".");
        byte[] fileBytes = Encoding.UTF8.GetBytes(File.ReadAllText(stashTargetFileInfo.FullName));
        CreateEdDataWorker(dbFileInfo.FullName, password).Stash(indexName, fileBytes);
    }
    public void Extract(FileInfo dbFileInfo, string indexName, string? nullablePassword)
    {
        if(!IsExists(dbFileInfo.FullName, ErrorDbFileDoesNotExists)) return;
        string password = nullablePassword ?? ConsoleCommon.ReadPassword();
        Console.WriteLine($"Do extract with name=\"{indexName}\", password=\"{password}\".");
        byte[] data = CreateEdDataWorker(dbFileInfo.FullName, password).Extract(indexName);
        string rawString = Encoding.UTF8.GetString(data);
        Console.WriteLine(rawString);
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
}
