namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public abstract class EdDataWorkerBase : IEdDataExtractor, IEdDataPlanter
{
    private IDatabaseOperator _dbOperator;
    private IEdDataCryptor _edCryptor;
    public EdDataWorkerBase(IDatabaseOperator dbOperator, IEdDataCryptor edCryptor)
    {
        _dbOperator = dbOperator;
        _edCryptor = edCryptor;
    }
    public virtual void Stash(string index, byte[] data, IMultipleKeyExchanger multiKey, byte[] key)
    {
        byte[] encryptedBytes = _edCryptor.EncryptBytes(data, multiKey);
        byte[] indexBytes = GenerateIndexBytes(index);
        _dbOperator.InsertData(indexBytes, encryptedBytes);
    }
    public virtual byte[] Extract(string index, IMultipleKeyExchanger multiKey, byte[] key)
    {
        byte[] indexBytes = GenerateIndexBytes(index);
        byte[] stashedBytes = _dbOperator.GetDataBytes(indexBytes);
        byte[] sourceBytes = _edCryptor.DecryptBytes(stashedBytes, multiKey);
        return sourceBytes;
    }
    protected abstract byte[] GenerateIndexBytes(string name);
}
