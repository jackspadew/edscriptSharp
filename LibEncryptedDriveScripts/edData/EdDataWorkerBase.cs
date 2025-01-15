namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public abstract class EdDataWorkerBase : IEdDataExtractor, IEdDataPlanter
{
    private IDatabaseOperator _dbOperator;
    private IEdDataCryptor _edCryptor;
    public IMultipleKeyExchanger _multipleKey;
    public EdDataWorkerBase(IDatabaseOperator dbOperator, IEdDataCryptor edCryptor)
    {
        _dbOperator = dbOperator;
        _edCryptor = edCryptor;
    }
    public virtual void Stash(string index, byte[] data)
    {
        byte[] encryptedBytes = _edCryptor.EncryptBytes(data, _multipleKey);
        byte[] indexBytes = GenerateIndexBytes(index);
        _dbOperator.InsertData(indexBytes, encryptedBytes);
    }
    public virtual byte[] Extract(string index)
    {
        byte[] indexBytes = GenerateIndexBytes(index);
        byte[] stashedBytes = _dbOperator.GetDataBytes(indexBytes);
        byte[] sourceBytes = _edCryptor.DecryptBytes(stashedBytes, _multipleKey);
        return sourceBytes;
    }
    protected abstract byte[] GenerateIndexBytes(string name);
}
