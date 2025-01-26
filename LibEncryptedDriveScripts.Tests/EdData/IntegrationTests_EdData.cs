namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;
using System.Text;

public class IntegrationTests_EdData
{
    private byte[] exampleBytes = new byte[]{0,1,2,3};
    private string examplePassword = "Example#Password";
    private string anotherPassword = "Another!!Password";
    private string dbPath = "IntegrationTests_EdData.db";
    private string exampleIndex = "IndexNameOfStashedData";
    private string notExistsIndex = "NonExistentIndexName";

    private IEdDataWorker DoCreateWorkerForTest(string password)
    {
        var logic = new BasicEdDataLogicFactory(dbPath, password);
        var worker = logic.CreateWorker();
        return worker;
    }

    private string GenerateRandomString(int length, int seed)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        StringBuilder sb = new StringBuilder(length);
        Random random = new Random(seed);
        for (int i = 0; i < length; i++)
        {
            int pos = random.Next(chars.Length);
            sb.Append(chars[pos]);
        }
        return sb.ToString();
    }
    private List<string> GenerateRandomStringList(int stringLength, int listCount)
    {
        Random random = new Random(0);
        var list = new List<string>();
        for(int i=0; i<listCount; i++)
        {
            string randomStr = GenerateRandomString(stringLength, random.Next());
            list.Add(randomStr);
        }
        return list;
    }

    [Fact]
    public void StashData_ItCanBeExtracted()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var workerForStash = DoCreateWorkerForTest(examplePassword);
        workerForStash.Stash(exampleIndex, exampleBytes);
        var workerForExtract = DoCreateWorkerForTest(examplePassword);
        byte[] extractedBytes = workerForExtract.Extract(exampleIndex);
        Assert.Equal(exampleBytes, extractedBytes);
    }

    [Fact]
    public void ExtractWithNotExistsIndex_Throw()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var workerForStash = DoCreateWorkerForTest(examplePassword);
        workerForStash.Stash(exampleIndex, exampleBytes);
        var workerForExtract = DoCreateWorkerForTest(examplePassword);
        Assert.Throws<InvalidOperationException>(() => workerForExtract.Extract(notExistsIndex));
    }

    [Fact]
    public void StashMultiplePiecesData_NotThrow()
    {
        int stashingCount = 10;
        CommonFunctions.DeleteFileIfExists(dbPath);
        var randomIndexNameList = GenerateRandomStringList(256, stashingCount);
        var workerForStash = DoCreateWorkerForTest(examplePassword);
        foreach(string str in randomIndexNameList)
        {
            workerForStash.Stash(str, exampleBytes);
        }
    }

    [Fact]
    public void StashBySameIndexNameAndDifferentPassword_IndexDoesNotCollide()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var worker = DoCreateWorkerForTest(examplePassword);
        worker.Stash(exampleIndex, exampleBytes);
        var workerWithAnotherPassword = DoCreateWorkerForTest(anotherPassword);
        workerWithAnotherPassword.Stash(exampleIndex, exampleBytes);
    }

    [Fact]
    public void ExtractWithAnotherPassword_Throw()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var workerForStash = DoCreateWorkerForTest(examplePassword);
        workerForStash.Stash(exampleIndex, exampleBytes);
        var workerForExtract = DoCreateWorkerForTest(anotherPassword);
        Assert.Throws<InvalidOperationException>(() => workerForExtract.Extract(exampleIndex));
    }

    [Fact]
    public void ExtractWithAnotherPassword_ExtractedDataIsNotEqual()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var workerForStash = DoCreateWorkerForTest(examplePassword);
        workerForStash.Stash(exampleIndex, exampleBytes);
        var workerForExtract = DoCreateWorkerForTest(anotherPassword);
        byte[] extractedBytes = workerForExtract.Extract(exampleIndex);
        Assert.NotEqual(exampleBytes, extractedBytes);
    }
}
