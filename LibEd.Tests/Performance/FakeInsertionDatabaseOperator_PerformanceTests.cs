namespace LibEd.PerformanceTests;

using LibEd.EdData;
using System.Reflection;
using LibEd.Tests;

#pragma warning disable CS8602

public class FakeInsertionDatabaseOperator_PerformanceTests
{
    private static byte[] exampleByte = new byte[256];
    private static byte[] exampleIndex = new byte[512];
    [Fact]
    public void InsertData_CompleteWithinTime()
    {
        string dbPath = MethodBase.GetCurrentMethod().Name + ".db";
        CommonFunctions.DeleteFileIfExists(dbPath);
        var dbOperator = new FakeInsertionDatabaseOperator(dbPath, true, 1000);
        PerformanceTestCommon.CompletesIn( MethodBase.GetCurrentMethod().Name, 5000, () => {
            dbOperator.InsertData(exampleIndex, exampleByte);
        });
    }
    [Fact]
    public void InsertDataByStream_CompleteWithinTime()
    {
        string dbPath = MethodBase.GetCurrentMethod().Name + ".db";
        CommonFunctions.DeleteFileIfExists(dbPath);
        MemoryStream stream = new(exampleByte);
        var dbOperator = new FakeInsertionDatabaseOperator(dbPath, true, 1000);
        PerformanceTestCommon.CompletesIn( MethodBase.GetCurrentMethod().Name, 5000, () => {
            dbOperator.InsertData(exampleIndex, stream);
        });
    }
}
