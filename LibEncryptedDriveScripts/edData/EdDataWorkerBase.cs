namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;
using System.Text;

public abstract class EdDataWorkerBase : IEdDataWorker
{
    private IDatabaseOperator _dbOperator;
    protected virtual IDatabaseOperator DbOperator {
        get {
            return _dbOperator;
        }
        }
    private IEdDataCryptor _edCryptor;
    protected virtual IEdDataCryptor EdCryptor {
        get {
            return _edCryptor;
        }
        }
    private IMultipleKeyExchanger _multipleKey;
    protected virtual IMultipleKeyExchanger MultipleKey {
        get {
            return _multipleKey;
        }
        }
    private IEdDataHashCalculator _hashCalculator;
    protected virtual IEdDataHashCalculator HashCalculator {
        get {
            return _hashCalculator;
        }
        }
    protected IEdDataLogicFactory _logicFactory;
    public EdDataWorkerBase(IEdDataLogicFactory logicFactory)
    {
        _logicFactory = logicFactory;
        _dbOperator = logicFactory.CreateDatabaseOperator();
        _edCryptor = logicFactory.CreateCryptor();
        _multipleKey = logicFactory.CreateMultipleKeyExchanger();
        _hashCalculator = logicFactory.CreateHashCalculator();
    }

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
    public bool IsIndexExists(string index)
    {
        byte[] indexBytes = GenerateIndexBytes(index);
        return DbOperator.IsIndexExists(indexBytes);
    }
    protected virtual byte[] GenerateIndexBytes(string name)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        byte[] rawIndexBytes = nameBytes;
        return _hashCalculator.ComputeHash(rawIndexBytes, MultipleKey);
    }
}
