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
}
