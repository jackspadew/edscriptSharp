namespace LibEncryptedDriveScripts.EdData;

using System.IO;
using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.Executor;
using System.Security.Cryptography;

public class EdDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    public EdDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {
    }
}

public class FakeInsertionDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    protected int InsertingFakeRowCount = 19;
    protected IActionExecutor RandomExecutor => new RandomizedExecutor([1,InsertingFakeRowCount]);
    public FakeInsertionDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {}
    public FakeInsertionDatabaseOperator(string dbPath, bool createFlag, int insertingFakeCount) : base(dbPath, createFlag)
    {
        InsertingFakeRowCount = insertingFakeCount;
    }

    public override void InsertData(byte[] index, byte[] data)
    {
        RandomExecutor.Run([
            () => base.InsertData(index, data),
            () => InsertFakeData(index, data)
        ]);
    }
    public override void InsertData(byte[] index, Stream readableStream)
    {
        RandomExecutor.Run([
            () => base.InsertData(index, readableStream),
            () => InsertFakeData(index, readableStream)
        ]);
    }
    protected virtual void InsertFakeData(byte[] imitatedIndex, byte[] imitatedData)
    {
        byte[] fakeIndex = GeneratedRandomBytes(imitatedIndex.Length);
        byte[] fakeData = GeneratedRandomBytes(imitatedData.Length);
        base.InsertData(fakeIndex, fakeData);
    }
    protected virtual void InsertFakeData(byte[] imitatedIndex, Stream readableStream)
    {
        byte[] fakeIndex = GeneratedRandomBytes(imitatedIndex.Length);
        byte[] fakeData = GeneratedRandomBytes(readableStream.Length);
        base.InsertData(fakeIndex, fakeData);
    }
    protected byte[] GeneratedRandomBytes(long length)
    {
        byte[] result = new byte[length];
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(result);
        return result;
    }
}
