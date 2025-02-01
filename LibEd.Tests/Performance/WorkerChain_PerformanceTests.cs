namespace LibEd.Tests;

using LibEd.EdData;
using System.Reflection;

#pragma warning disable CS8602

public class PerformanceTests_WorkerChain
{
    private static byte[] exampleByte = new byte[]{0,1,2,3};
    private static string exampleIndex = "example";
    private static string examplePassword = "password";
    [Fact]
    public void StashWithLongChain_CompleteWithinTime()
    {
        string dbPath = MethodBase.GetCurrentMethod().Name + ".db";
        CommonFunctions.DeleteFileIfExists(dbPath);
        int targetChainDepth = 3;
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var workerChainZero = new EdDataWorkerChain(logic, initialWorker);
        IEdDataWorkerChain nextWorker = workerChainZero;
        for(int i=0; i<targetChainDepth; i++)
        {
            nextWorker = new EdDataWorkerChain(logic, nextWorker);
        }
        var workerLast = nextWorker;
        byte[] extracted = new byte[0];
        CommonFunctions.CompletesIn(5000, () => {
            workerLast.Stash(exampleIndex, exampleByte);
        });
        extracted = workerLast.Extract(exampleIndex);
        Assert.Equal(exampleByte, extracted);
    }

    [Fact]
    public void ExtractWithLongChain_CompleteWithinTime()
    {

        string dbPath = MethodBase.GetCurrentMethod().Name + ".db";
        CommonFunctions.DeleteFileIfExists(dbPath);
        int targetChainDepth = 3;
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var workerChainZero = new EdDataWorkerChain(logic, initialWorker);
        IEdDataWorkerChain nextWorker = workerChainZero;
        for(int i=0; i<targetChainDepth; i++)
        {
            nextWorker = new EdDataWorkerChain(logic, nextWorker);
        }
        var workerLast = nextWorker;
        byte[] extracted = new byte[0];
        workerLast.Stash(exampleIndex, exampleByte);
        CommonFunctions.CompletesIn(1000, () => {
            extracted = workerLast.Extract(exampleIndex);
        });
        Assert.Equal(exampleByte, extracted);
    }
}
