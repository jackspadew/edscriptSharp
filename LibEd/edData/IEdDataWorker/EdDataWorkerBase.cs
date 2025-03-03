namespace LibEd.EdData;

using LibEd.Database;
using System.Text;

public abstract class EdDataWorkerBase : IEdDataWorker
{
    protected virtual IDatabaseOperator DbOperator => _logicFactory.CreateDatabaseOperator(this);
    protected virtual IEdDataCryptor EdCryptor => _logicFactory.CreateCryptor(this);
    protected virtual IEdDataHashCalculator HashCalculator => _logicFactory.CreateHashCalculator(this);
    protected abstract IMultipleKeyExchanger MultipleKey {get;}
    protected IEdDataLogicFactory _logicFactory;

    public EdDataWorkerBase(IEdDataLogicFactory logicFactory)
    {
        _logicFactory = logicFactory;
    }

    public virtual void Stash(string index, byte[] data)
    {
        byte[] encryptedBytes = EdCryptor.EncryptBytes(data, MultipleKey);
        byte[] indexBytes = GenerateIndexBytes(index);
        try{
            DbOperator.InsertData(indexBytes, encryptedBytes);
        }
        catch (Exception ex)
        {
            string IndexBytesString = BitConverter.ToString(GenerateIndexBytes(index));
            string OwnMultipleKeyString = MultipleKey.ToString();
            throw new Exception($"{this.GetType().Name}: IndexString={index}, IndexBytes={IndexBytesString}, MultipleKey={OwnMultipleKeyString}", ex);
        }
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
        return HashCalculator.ComputeHash(rawIndexBytes, MultipleKey);
    }
}
