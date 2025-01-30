namespace LibEncryptedDriveScripts.EdData;

using System.IO;
using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.Executor;

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
    }
    protected virtual void InsertFakeData(byte[] imitatedIndex, Stream readableStream)
    {
    }
}
