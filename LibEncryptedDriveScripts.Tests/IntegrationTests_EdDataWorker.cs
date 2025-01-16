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
    private static string exampleDataName = "example";

    [Fact]
    public void CreateWorker_ReturnEdDataKeyMakingWorker()
    {
        DeleteFileIfExists(dbPath);
        var initWorker = new EdDataInitialWorker(dbPath);
        var createdWorker = initWorker.NextWorker();
        bool IsEdDataWorker = createdWorker is EdDataKeyMakingWorker;
        Assert.True(IsEdDataWorker);
    }

    [Fact]
    public void StashAndExtractByEdDataKeyMakingWorker_ReturnSameBytes()
    {
        DeleteFileIfExists(dbPath);
        var initWorker = new EdDataInitialWorker(dbPath);
        var keymakeWorker = initWorker.NextWorker();
        var exampleMultiKey = new KeyMakerMultipleKeyExchanger();
        byte[] exampleMultiKeyBytes = exampleMultiKey.GetBytes();
        keymakeWorker.Stash(exampleDataName, exampleMultiKeyBytes);
        byte[] extractKeyBytes = keymakeWorker.Extract(exampleDataName);
        Assert.Equal(exampleMultiKeyBytes, extractKeyBytes);
    }
}
