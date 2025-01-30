namespace LibEncryptedDriveScripts.EdData;

using System.IO;
using LibEncryptedDriveScripts.Database;

public class EdDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    public EdDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {}

    public override void InsertData(byte[] index, byte[] data)
    {
        base.InsertData(index, data);
    }
    public override void InsertData(byte[] index, Stream readableStream)
    {
        base.InsertData(index, readableStream);
    }
}
