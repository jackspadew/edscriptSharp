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

    [Fact]
    public void CreateDatabaseOperatorInstanceWithoutCreateFlag_Throw()
    {
        string dbPathNonExistentFile = "nonexistent.db";
        if(File.Exists(dbPathNonExistentFile))
        {
            File.Delete(dbPathNonExistentFile);
        }
        Assert.Throws<FileNotFoundException>( () => new DatabaseOperator_ForTest(dbPathNonExistentFile, false) );
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertDataByBytes_NotThrow(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = new byte[8];
        byte[] exampleData = new byte[16];
        dbOperator.InsertData(exampleIndex, exampleData);
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertDataByStream_NotThrow(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = {0,0,0,2};
        byte[] exampleData = new byte[16];
        MemoryStream mStream = new();
        mStream.Write(exampleData, 0, exampleData.Length);
        dbOperator.InsertData(exampleIndex, mStream);
    }

    public void Dispose()
    {
        if(File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }
    }
}
