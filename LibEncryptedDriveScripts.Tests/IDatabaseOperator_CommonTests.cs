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

    public static string dbPathDbOpeBaseForTestPath = "example.db";
    public static byte[] exampleBytes = {0,1,2,3};
    public static IEnumerable<object[]> IDatabaseOperatorObjects()
    {
        yield return new object[] { new DatabaseOperator_ForTest(dbPathDbOpeBaseForTestPath, true), "DatabaseOperator_ForTest" };
    }

    private void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CreateDatabaseOperatorInstance_DbFileCreated()
    {
        string dbPathCreateTest = "createtest.db";
        DeleteFileIfExists(dbPathCreateTest);
        DatabaseOperator_ForTest dbOperator = new (dbPathCreateTest, true);
        Assert.True(File.Exists(dbPathCreateTest), "The database file was not created.");
        File.Delete(dbPathCreateTest);
    }

    [Fact]
    public void CreateDatabaseOperatorInstanceWithoutCreateFlag_Throw()
    {
        string dbPathNonExistentFile = "nonexistent.db";
        DeleteFileIfExists(dbPathNonExistentFile);
        Assert.Throws<FileNotFoundException>( () => new DatabaseOperator_ForTest(dbPathNonExistentFile, false) );
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertDataByBytes_NotThrow(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = {0,0,0,1};
        dbOperator.InsertData(exampleIndex, exampleBytes);
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertDataByStream_NotThrow(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = {0,0,0,2};
        MemoryStream mStream = new();
        mStream.Write(exampleBytes, 0, exampleBytes.Length);
        dbOperator.InsertData(exampleIndex, mStream);
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertThenReadBytes_ReturnEqualBytes(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = {0,0,0,3};
        byte[] exampleBytes = {1,1,1,1};
        dbOperator.InsertData(exampleIndex, exampleBytes);
        byte[] result = dbOperator.GetDataBytes(exampleIndex);
        bool isEqual = exampleBytes.SequenceEqual(result);
        Assert.True(isEqual);
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertThenReadBytesByStream_ReturnEqualBytes(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = {0,0,0,4};
        byte[] exampleBytes = {2,2,2,2};
        dbOperator.InsertData(exampleIndex, exampleBytes);
        Stream stream = dbOperator.GetDataStream(exampleIndex);
        byte[] result = new byte[stream.Length];
        stream.Read(result, 0, result.Length);
        bool isEqual = exampleBytes.SequenceEqual(result);
        Assert.True(isEqual);
    }

    [Theory]
    [MemberData(nameof(IDatabaseOperatorObjects))]
    public void InsertDataThenCheckExists_ReturnTrue(IDatabaseOperator dbOperator, string className)
    {
        byte[] exampleIndex = {0,0,0,6};
        dbOperator.InsertData(exampleIndex, exampleBytes);
        bool result = dbOperator.IsIndexExists(exampleIndex);
        Assert.True(result);
    }

    public void Dispose()
    {
        DeleteFileIfExists(dbPathDbOpeBaseForTestPath);
    }
}
