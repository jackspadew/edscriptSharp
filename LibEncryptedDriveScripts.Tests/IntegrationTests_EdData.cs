namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;

public class IntegrationTests_EdData
{
    private byte[] exampleBytes = new byte[]{0,1,2,3};
    private string examplePassword = "Example#Password";
    private string dbPath = "IntegrationTests_EdData.db";
    private string exampleIndex = "IndexNameOfStashedData";
    private string notExistsIndex = "NonExistentIndexName";

    private IEdDataWorker DoCreateWorkerForTest()
    {
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        var worker = logic.CreateWorker();
        return worker;
    }

    [Fact]
    public void StashData_ItCanBeExtracted()
    {
        var workerForStash = DoCreateWorkerForTest();
        workerForStash.Stash(exampleIndex, exampleBytes);
        var workerForExtract = DoCreateWorkerForTest();
        byte[] extractedBytes = workerForExtract.Extract(exampleIndex);
        Assert.Equal(exampleBytes, extractedBytes);
    }
}
