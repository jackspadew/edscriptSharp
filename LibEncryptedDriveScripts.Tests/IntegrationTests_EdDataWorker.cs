namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.EdData;

public class IntegrationTests_EdDataWorker
{

    private void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }
    private static string dbPath = "IntegrationTests_EdDataWorker.db";

    [Fact]
    public void CreateWorker_ReturnEdDataKeyMakingWorker()
    {
        DeleteFileIfExists(dbPath);
        var initWorker = new EdDataInitialWorker(dbPath);
        var createdWorker = initWorker.NextWorker();
        bool IsEdDataWorker = createdWorker is EdDataKeyMakingWorker;
        Assert.True(IsEdDataWorker);
    }
}
