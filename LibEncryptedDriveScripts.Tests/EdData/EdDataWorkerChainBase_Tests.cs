namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;
using Moq;

public class EdDataWorkerChainBase_Tests
{
    private static string dbPath = "EdDataWorkerChainBase_Tests.db";
    private static string exampleIndex = "example";
    private static string examplePassword = "password";
    private static byte[] exampleByte = new byte[]{0,1,2,3};

    [Fact]
    public void StashThenExtractWithTwoChain_ReturnSameBytes()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var workerChainZero = new EdDataWorkerChain(logic, initialWorker);
        var workerLast = new EdDataWorkerChain(logic, workerChainZero);
        workerLast.Stash(exampleIndex, exampleByte);
        var extracted = workerLast.Extract(exampleIndex);
        Assert.Equal(exampleByte, extracted);
    }

    [Fact]
    public void StashChildMultipleKeyThenExtract_MultipleKeyBytesAreNotEqual()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var workerChainZero = new EdDataWorkerChain(logic, initialWorker);
        var workerSecond = new EdDataWorkerChain(logic, workerChainZero);
        workerSecond.StashChildMultipleKey(exampleIndex);
        var keyLast = workerSecond.ExtractChildMultipleKey(exampleIndex);
        var keySecond = workerChainZero.ExtractChildMultipleKey(exampleIndex);
        Assert.NotEqual(keyLast.GetBytes(), keySecond.GetBytes());
    }

    [Fact]
    public void StashThenExtractWithThreeChain_ReturnSameBytes()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var workerChainZero = new EdDataWorkerChain(logic, initialWorker);
        var workerSecond = new EdDataWorkerChain(logic, workerChainZero);
        var workerLast = new EdDataWorkerChain(logic, workerSecond);
        workerLast.Stash(exampleIndex, exampleByte);
        var extracted = workerLast.Extract(exampleIndex);
        Assert.Equal(exampleByte, extracted);
    }

    [Fact]
    public void CallStashOfLastWorker_CountOfCallingStashChildMultipleKeyIsCorrect()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var mockedWorkerChainZero = new Mock<EdDataWorkerChainBase>(logic, initialWorker){ CallBase = true };
        var mockedWorkerLast = new Mock<EdDataWorkerChainBase>(logic, mockedWorkerChainZero.Object){ CallBase = true };
        mockedWorkerLast.Object.Stash(exampleIndex, exampleByte);
        var extracted = mockedWorkerLast.Object.Extract(exampleIndex);
        mockedWorkerChainZero.Verify(s => s.StashChildMultipleKey(It.IsAny<string>()), Times.Once);
        mockedWorkerLast.Verify(s => s.StashChildMultipleKey(It.IsAny<string>()), Times.Never);
    }
}
