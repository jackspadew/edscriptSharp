namespace edcli;

using LibEncryptedDriveScripts.EdData;
using System.Text;

public partial class Program
{
    public static IEdDataWorker CreateEdDataWorker(string dbPath, string password)
    {
        IEdDataLogicFactory logic = new BasicEdDataLogicFactory(dbPath, password);
        IEdDataWorker worker = logic.CreateWorker();
        return worker;
    }
    public static void Stash(FileInfo dbFileInfo, string indexName, FileInfo stashTargetFileInfo, string? nullablePassword)
    {
        string password = nullablePassword ?? ReadPassword();
        Console.WriteLine($"Do stash with name=\"{indexName}\", file=\"{stashTargetFileInfo.FullName}\" password=\"{password}\".");
        byte[] fileBytes = Encoding.UTF8.GetBytes(File.ReadAllText(stashTargetFileInfo.FullName));
        CreateEdDataWorker(dbFileInfo.FullName, password).Stash(indexName, fileBytes);
    }
    public static void Extract(FileInfo dbFileInfo, string indexName, string? nullablePassword)
    {
        string password = nullablePassword ?? ReadPassword();
        Console.WriteLine($"Do extract with name=\"{indexName}\", password=\"{password}\".");
        byte[] data = CreateEdDataWorker(dbFileInfo.FullName, password).Extract(indexName);
        string rawString = Encoding.UTF8.GetString(data);
        Console.WriteLine(rawString);
    }
}
