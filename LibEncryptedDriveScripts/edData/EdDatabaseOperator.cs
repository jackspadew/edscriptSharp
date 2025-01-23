namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public class EdDatabaseOperator : DatabaseOperatorBase, IDatabaseOperator
{
    public EdDatabaseOperator(string dbPath, bool createFlag) : base(dbPath, createFlag)
    {}
}
