namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.Database;

#pragma warning disable xUnit1026 // Unused arguments

public class IDatabaseOperator_CommonTests : IDisposable
{
    public class DatabaseOperator_ForTest : DatabaseOperatorBase, IDatabaseOperator
    {
        public DatabaseOperator_ForTest(string dbPath, bool createFlag) : base(dbPath, createFlag) {}
        public DatabaseOperator_ForTest(string dbPath) : base(dbPath, true) {}
    }

    public static string dbPath = "example.db";
    public static IEnumerable<object[]> IDatabaseOperatorObjects()
    {
        yield return new object[] { new DatabaseOperator_ForTest(dbPath, true), "DatabaseOperator_ForTest" };
    }

    [Fact]
    public void CreateDatabaseOperatorInstance_DbFileCreated()
    {
        string dbPathCreateTest = "createtest.db";
        if(File.Exists(dbPathCreateTest))
        {
            File.Delete(dbPathCreateTest);
        }
        DatabaseOperator_ForTest dbOperator = new (dbPathCreateTest, true);
        Assert.True(File.Exists(dbPathCreateTest), "The database file was not created.");
        File.Delete(dbPathCreateTest);
    }

    public void Dispose()
    {
        if(File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }
    }
}
