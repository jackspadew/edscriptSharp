namespace LibEd.PerformanceTests;

using LibEd.EdData;
using System.Reflection;
using LibEd.Tests;

#pragma warning disable CS8602

public class PerformanceTests_WorkerChain
{
    private static byte[] exampleByte = new byte[]{0,1,2,3};
    private static string exampleIndex = "example";
    private static string examplePassword = "password";
    [SkippableFact]
    public void StashWithLongChain_CompleteWithinTime()
    {
        Skip.IfNot(PerformanceTestCommon.EnablePerformanceTests, PerformanceTestCommon.MessageWhenDisableTests);
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
        PerformanceTestCommon.CompletesIn(nameof(StashWithLongChain_CompleteWithinTime), 15000, () => {
            workerLast.Stash(exampleIndex, exampleByte);
        });
        extracted = workerLast.Extract(exampleIndex);
        Assert.Equal(exampleByte, extracted);
    }

    [SkippableFact]
    public void ExtractWithLongChain_CompleteWithinTime()
    {
        Skip.IfNot(PerformanceTestCommon.EnablePerformanceTests, PerformanceTestCommon.MessageWhenDisableTests);
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
        PerformanceTestCommon.CompletesIn(nameof(ExtractWithLongChain_CompleteWithinTime), 1000, () => {
            extracted = workerLast.Extract(exampleIndex);
        });
        Assert.Equal(exampleByte, extracted);
    }

    [SkippableFact]
    public void Stash_CompleteWithinTime()
    {
        Skip.IfNot(PerformanceTestCommon.EnablePerformanceTests, PerformanceTestCommon.MessageWhenDisableTests);
        string dbPath = MethodBase.GetCurrentMethod().Name + ".db";
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        var workerLast = logic.CreateWorker();
        byte[] extracted = new byte[0];
        PerformanceTestCommon.CompletesIn(nameof(StashWithLongChain_CompleteWithinTime), 10000, () => {
            workerLast.Stash(exampleIndex, exampleByte);
        });
        extracted = workerLast.Extract(exampleIndex);
        Assert.Equal(exampleByte, extracted);
    }

    [SkippableFact]
    public void Extract_CompleteWithinTime()
    {
        Skip.IfNot(PerformanceTestCommon.EnablePerformanceTests, PerformanceTestCommon.MessageWhenDisableTests);
        string dbPath = MethodBase.GetCurrentMethod().Name + ".db";
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        var workerLast = logic.CreateWorker();
        byte[] extracted = new byte[0];
        workerLast.Stash(exampleIndex, exampleByte);
        PerformanceTestCommon.CompletesIn(nameof(ExtractWithLongChain_CompleteWithinTime), 1000, () => {
            extracted = workerLast.Extract(exampleIndex);
        });
        Assert.Equal(exampleByte, extracted);
    }
}
