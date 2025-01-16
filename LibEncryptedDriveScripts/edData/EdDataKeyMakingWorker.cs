namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.HashCalculator;

public class EdDataKeyMakingWorker : EdDataWorkerBase, IEdDataWorker
{
    private readonly int MultipleEncryptionCount = 1000;
    private readonly int HashStretchingCount = 1000;
    private IDatabaseOperator _dbOperator;
    protected override IDatabaseOperator DbOperator { get => _dbOperator; }
    private IEdDataCryptor _edCryptor;
    protected override IEdDataCryptor EdCryptor { get => _edCryptor; }
    private IMultipleKeyExchanger _multipleKey;
    protected override IMultipleKeyExchanger MultipleKey { get => _multipleKey; }
    private IHashCalculator _hashCalculator;
    public EdDataKeyMakingWorker(IDatabaseOperator dbOperator)
    {
        _dbOperator = dbOperator;
        _multipleKey = new KeyMakerMultipleKeyExchanger();
        _edCryptor = new EdDataCryptor(MultipleEncryptionCount);
        _hashCalculator = new RandomizedHashCalculator(new byte[64], MultipleKey.HashSeed);
    }
    protected override byte[] GenerateIndexBytes(string name)
    {
        throw new NotImplementedException();
    }
}

public class KeyMakerMultipleKeyExchanger : MultipleKeyExchangerBase, IMultipleKeyExchanger
{}
