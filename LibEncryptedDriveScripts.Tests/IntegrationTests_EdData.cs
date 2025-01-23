namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;
using System.Text;

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
        var workerForStash = DoCreateWorkerForTest();
        workerForStash.Stash(exampleIndex, exampleBytes);
        var workerForExtract = DoCreateWorkerForTest();
        byte[] extractedBytes = workerForExtract.Extract(exampleIndex);
        Assert.Equal(exampleBytes, extractedBytes);
    }

    [Fact]
    public void StashMultiplePiecesData_NotThrow()
    {
        int stashingCount = 10;
        CommonFunctions.DeleteFileIfExists(dbPath);
        var randomIndexNameList = GenerateRandomStringList(256, stashingCount);
        var workerForStash = DoCreateWorkerForTest();
        foreach(string str in randomIndexNameList)
        {
            workerForStash.Stash(str, exampleBytes);
        }
    }
}
