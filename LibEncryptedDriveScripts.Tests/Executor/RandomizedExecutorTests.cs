namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.Executor;
using Moq;

public class RandomizedExecutor_Tests
{
    [Fact]
    public void RunWithSpecifiedExecutionCount_CalledCorrectTimes()
    {
        int[] countArray = new int[]{1,2};
        var mockedActOne = new Mock<Action>();
        var mockedActTwo = new Mock<Action>();
        var executor = new RandomizedExecutor(countArray);
        executor.Run([mockedActOne.Object,mockedActTwo.Object]);
        mockedActOne.Verify(a => a(), Times.Exactly(countArray[0]));
        mockedActTwo.Verify(a => a(), Times.Exactly(countArray[1]));
    }

    [Fact]
    public void RunWithLongCountList_CalledCorrectTimes()
    {
        int[] countArray = new int[]{0,2,9,9,9};
        var mockedActOne = new Mock<Action>();
        var mockedActTwo = new Mock<Action>();
        var executor = new RandomizedExecutor(countArray);
        executor.Run([mockedActOne.Object,mockedActTwo.Object]);
        mockedActOne.Verify(a => a(), Times.Exactly(countArray[0]));
        mockedActTwo.Verify(a => a(), Times.Exactly(countArray[1]));
    }

    [Fact]
    public void RunWithShortCountList_CalledCorrectTimes()
    {
        int[] countArray = new int[]{2};
        var mockedActOne = new Mock<Action>();
        var mockedActTwo = new Mock<Action>();
        var executor = new RandomizedExecutor(countArray);
        executor.Run([mockedActOne.Object,mockedActTwo.Object]);
        mockedActOne.Verify(a => a(), Times.Exactly(countArray[0]));
        mockedActTwo.Verify(a => a(), Times.Exactly(0));
    }

    [Fact]
    public void Run_ExecutionOrderIsRandomized()
    {
        int markNumber = 100;
        int[] countArray = new int[]{1,99999};
        List<int> stashTargetList = new();
        var executor = new RandomizedExecutor(countArray);
        executor.Run([
            () => {stashTargetList.Add(markNumber);},
            () => {stashTargetList.Add(0);},
            ]);
        int markNumberCount = stashTargetList.Count(x => x == markNumber);
        int markNumberIndex = stashTargetList.IndexOf(markNumber);
        Assert.Equal(1, markNumberCount);
        Assert.NotEqual(0, markNumberIndex);
    }
}
