namespace LibEncryptedDriveScripts.Tests;

using System.Text;
using LibEncryptedDriveScripts.EdData;
using Moq;
using Moq.Protected;

public class EdDataWorkerChainBase_Tests
{
    private static string dbPath = "EdDataWorkerChainBase_Tests.db";
    private static string exampleIndex = "example";
    private static string anotherIndex = "anotherIndex";
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

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CallStashOfLastWorker_CountOfCallingStashChildMultipleKeyIsCorrect(int targetChainDepth)
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var mockedWorkerChainZero = new Mock<EdDataWorkerChainBase>(logic, initialWorker){ CallBase = true };
        List<Mock<EdDataWorkerChainBase>> mockedWorkerList = new();
        Mock<EdDataWorkerChainBase> workerNextParent = mockedWorkerChainZero;
        for(int i=0; i<targetChainDepth-1; i++)
        {
            workerNextParent = new Mock<EdDataWorkerChainBase>(logic, workerNextParent.Object){ CallBase = true };
            mockedWorkerList.Add(workerNextParent);
        }
        var mockedWorkerLast = new Mock<EdDataWorkerChainBase>(logic, workerNextParent.Object){ CallBase = true };
        mockedWorkerLast.Object.Stash(exampleIndex, exampleByte);
        var extracted = mockedWorkerLast.Object.Extract(exampleIndex);
        mockedWorkerChainZero.Verify(s => s.StashChildMultipleKey(It.IsAny<string>()), Times.Once);
        mockedWorkerLast.Verify(s => s.StashChildMultipleKey(It.IsAny<string>()), Times.Never);
        foreach(var mockedWorker in mockedWorkerList)
        {
            mockedWorker.Verify(s => s.StashChildMultipleKey(It.IsAny<string>()), Times.Once);
        }
    }

    [Fact]
    public void StashThenExtractWithLongChain_CompleteWithinTime()
    {
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
            extracted = workerLast.Extract(exampleIndex);
        });
        Assert.Equal(exampleByte, extracted);
    }

    [Fact]
    public void WhenIndexbytesCollides_RegeneratedMultipleKey()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        byte[] ItWillBeCollided = new byte[]{1,0,0,0};
        byte[] NotCollided = new byte[]{2,0,0,0};
        var logic = new BasicEdDataLogicFactory(dbPath, examplePassword);
        var mockedHashCalculator = new Mock<IEdDataHashCalculator>();
        mockedHashCalculator.Setup(a => a.ComputeHash(Encoding.UTF8.GetBytes(exampleIndex),It.IsAny<IMultipleKeyExchanger>()))
            .Returns(ItWillBeCollided); // IndexBytes for first Stashing.
        mockedHashCalculator.SetupSequence(a => a.ComputeHash(Encoding.UTF8.GetBytes(anotherIndex),It.IsAny<IMultipleKeyExchanger>()))
            .Returns(ItWillBeCollided) // Call within IsIndexExists in StashChildMultipleKey
            .Returns(ItWillBeCollided) // Call within IsIndexExists of while conditional formula in RegenerateOwnMultipleKey
            .Returns(ItWillBeCollided) // Call in RegenerateOwnMultipleKey after Regenerate (duplicate again)
            .Returns(NotCollided) // Call in RegenerateOwnMultipleKey after Regenerate
            .Returns(NotCollided); // Call in base.Stash
        var mockedLogic = new Mock<BasicEdDataLogicFactory>(dbPath, examplePassword){ CallBase = true };
        mockedLogic.Setup(a => a.CreateHashCalculator(It.IsAny<IEdDataWorker>())).Returns(mockedHashCalculator.Object);
        IEdDataWorker initialWorker = new EdDataInitialWorker(logic);
        var workerChainZero = new Mock<EdDataWorkerChainBase>(logic, initialWorker){ CallBase = true };
        int expectedCount_RegenerateChildMultipleKey = 0;
        workerChainZero.Setup(s => s.RegenerateChildMultipleKey(anotherIndex)).Callback(() => {
            expectedCount_RegenerateChildMultipleKey += 1;
        });
        var workerSecond = new Mock<EdDataWorkerChainBase>(mockedLogic.Object, workerChainZero.Object){ CallBase = true };
        workerSecond.Object.Stash(exampleIndex, exampleByte);
        workerSecond.Object.Stash(anotherIndex, exampleByte);
        int actualCount_RegenerateOwnMultipleKeys = 1;
        workerSecond.Protected().Verify("RegenerateOwnMultipleKey", Times.Exactly(actualCount_RegenerateOwnMultipleKeys), new object[]{ItExpr.IsAny<string>()});
        int actualCount_RegenerateChildMultipleKey = 2;
        workerChainZero.Verify(s => s.RegenerateChildMultipleKey(It.IsAny<string>()), Times.Exactly(actualCount_RegenerateChildMultipleKey));
        Assert.Equal(actualCount_RegenerateChildMultipleKey, expectedCount_RegenerateChildMultipleKey);
        int actualCount_GenerateIndexBytesForAnotherIndex = 5;
        workerSecond.Protected().Verify("GenerateIndexBytes", Times.Exactly(actualCount_GenerateIndexBytesForAnotherIndex), anotherIndex);
    }
}
