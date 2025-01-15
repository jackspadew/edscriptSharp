namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public class EdDataInitializer : EdDataWorkerBase, IEdDataExtractor, IEdDataPlanter
{
    public EdDataInitializer(IDatabaseOperator dbOperator, IEdDataCryptor edCryptor) : base(dbOperator, edCryptor)
    {}

    protected override byte[] GenerateIndexBytes(string name)
    {
        throw new NotImplementedException();
    }
}
