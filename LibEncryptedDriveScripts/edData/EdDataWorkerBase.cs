namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public abstract class EdDataWorkerBase : IEdDataExtractor, IEdDataPlanter
{
    protected abstract IDatabaseOperator DbOperator {get;}
    protected abstract IEdDataCryptor EdCryptor {get;}
    protected abstract IMultipleKeyExchanger MultipleKey {get;}

    public virtual void Stash(string index, byte[] data)
    {
        byte[] encryptedBytes = EdCryptor.EncryptBytes(data, MultipleKey);
        byte[] indexBytes = GenerateIndexBytes(index);
        DbOperator.InsertData(indexBytes, encryptedBytes);
    }
    public virtual byte[] Extract(string index)
    {
        byte[] indexBytes = GenerateIndexBytes(index);
        byte[] stashedBytes = DbOperator.GetDataBytes(indexBytes);
        byte[] sourceBytes = EdCryptor.DecryptBytes(stashedBytes, MultipleKey);
        return sourceBytes;
    }
    protected abstract byte[] GenerateIndexBytes(string name);
}
