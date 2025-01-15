namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public class EdDataInitializer : EdDataWorkerBase, IEdDataExtractor, IEdDataPlanter
{
    private readonly int MultipleEncryptionCount = 1000;
    private IDatabaseOperator _dbOperator;
    protected override IDatabaseOperator DbOperator { get => _dbOperator; }
    private IEdDataCryptor _edCryptor;
    protected override IEdDataCryptor EdCryptor { get => _edCryptor; }
    private IMultipleKeyExchanger _multipleKey;
    protected override IMultipleKeyExchanger MultipleKey { get => _multipleKey; }

    public EdDataInitializer(string dbPath)
    {
        _dbOperator = new EdDatabaseOperator(dbPath, true);
        _multipleKey = new InitialMultipleKeyExchanger();
        _edCryptor = new EdDataCryptor(MultipleEncryptionCount);
    }
    protected override byte[] GenerateIndexBytes(string name)
    {
        throw new NotImplementedException();
    }
}

public class InitialMultipleKeyExchanger : MultipleKeyExchangerBase, IMultipleKeyExchanger
{
    public InitialMultipleKeyExchanger()
    {
        Random random = new Random(20626197);
        KeySeed = 90849388;
        IVSeed = 88871264;
        AlgorithmSeed = 93476436;
        byte[] key = new byte[32];
        random.NextBytes(key);
        byte[] iv = new byte[16];
        random.NextBytes(iv);
        Key = key;
        IV = iv;
    }
}
