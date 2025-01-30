namespace LibEncryptedDriveScripts.EdData;

using System.IO;
using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.Executor;
using System.Security.Cryptography;

public class EdDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    protected int InsertingFakeRowCount = 99;
    protected IActionExecutor RandomExecutor => new RandomizedExecutor([1,InsertingFakeRowCount]);
    public EdDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {}
    public EdDatabaseOperator(string dbPath, bool createFlag, int insertingFakeCount) : base(dbPath, createFlag)
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
