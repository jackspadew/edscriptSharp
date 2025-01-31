namespace LibEd.Tests;

using LibEd.EdData;
using Xunit;
using Moq;
using Moq.Protected;

public class FakeInsertionDatabaseOperator_Tests
{
    private static string dbPath = "FakeInsertionDatabaseOperator_Tests.db";
    private static byte[] exampleIndexBytes = new byte[]{255,0,0,1};
    private static byte[] exampleData = new byte[]{0,1,2,3,4};

    [Fact]
    public void InsertData_CallingInsertFakeDataCountIsCorrect()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        int executionCount = 9;
        var mockedDbOperator = new Mock<FakeInsertionDatabaseOperator>(dbPath, true, executionCount){ CallBase = true };
        mockedDbOperator.Object.InsertData(exampleIndexBytes, exampleData);
        int actualCount = executionCount;
        mockedDbOperator.Protected().Verify("InsertFakeData", Times.Exactly(actualCount), [exampleIndexBytes, exampleData]);
    }

    [Fact]
    public void InsertDataByStream_CallingInsertFakeDataCountIsCorrect()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        int executionCount = 9;
        var mockedDbOperator = new Mock<FakeInsertionDatabaseOperator>(dbPath, true, executionCount){ CallBase = true };
        var streamData = new MemoryStream(exampleData);
        mockedDbOperator.Object.InsertData(exampleIndexBytes, streamData);
        int actualCount = executionCount;
        mockedDbOperator.Protected().Verify("InsertFakeData", Times.Exactly(actualCount), [exampleIndexBytes, streamData]);
    }
}
