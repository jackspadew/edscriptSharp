namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;
using Xunit;
using Moq;
using Moq.Protected;

public class EdDatabaseOperator_Tests
{
    private static string dbPath = "EdDatabaseOperator_Tests.db";
    private static byte[] exampleIndexBytes = new byte[]{255,0,0,1};
    private static byte[] exampleData = new byte[]{0,1,2,3,4};

    [Fact]
    public void InsertData_CallingInsertFakeDataCountIsCorrect()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        int executionCount = 9;
        var mockedDbOperator = new Mock<EdDatabaseOperator>(dbPath, true, executionCount){ CallBase = true };
        mockedDbOperator.Object.InsertData(exampleIndexBytes, exampleData);
        int actualCount = executionCount;
        mockedDbOperator.Protected().Verify("InsertFakeData", Times.Exactly(actualCount), [exampleIndexBytes, exampleData]);
    }

    [Fact]
    public void InsertDataByStream_CallingInsertFakeDataCountIsCorrect()
    {
        CommonFunctions.DeleteFileIfExists(dbPath);
        int executionCount = 9;
        var mockedDbOperator = new Mock<EdDatabaseOperator>(dbPath, true, executionCount){ CallBase = true };
        var streamData = new MemoryStream(exampleData);
        mockedDbOperator.Object.InsertData(exampleIndexBytes, streamData);
        int actualCount = executionCount;
        mockedDbOperator.Protected().Verify("InsertFakeData", Times.Exactly(actualCount), [exampleIndexBytes, streamData]);
    }
}
