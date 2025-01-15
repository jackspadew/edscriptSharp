namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.EdData;
using LibEncryptedDriveScripts.Database;

public class EdDataInitializer_Tests
{
    public static string dbPath = "EdDataInitializer_Tests.db";

    private IDatabaseOperator GetDatabaseOperator()
    {
        return new EdDatabaseOperator(dbPath, true);
    }

    private void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CreateObject_InitialMultiKeyIsExists()
    {
        DeleteFileIfExists(dbPath);
        var edWorker = new EdDataInitializer(dbPath);
        bool IsInitialMultiKeyExists = edWorker.IsIndexExists("__InitialMultiKey");
        Assert.True(IsInitialMultiKeyExists);
    }
}
