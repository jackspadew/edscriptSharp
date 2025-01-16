namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.HashCalculator;
using System.Text;

public class EdDataInitialWorker : EdDataWorkerBase, IEdDataExtractor, IEdDataPlanter
{
    private readonly int MultipleEncryptionCount = 1000;
    private readonly int HashStretchingCount = 1000;
    private readonly string InitialMultipleKeyIndexName = "__InitialMultiKey";
    private IDatabaseOperator _dbOperator;
    protected override IDatabaseOperator DbOperator { get => _dbOperator; }
    private IEdDataCryptor _edCryptor;
    protected override IEdDataCryptor EdCryptor { get => _edCryptor; }
    private IMultipleKeyExchanger _multipleKey;
    protected override IMultipleKeyExchanger MultipleKey { get => _multipleKey; }
    private IHashCalculator _hashCalculator;

    public EdDataInitialWorker(string dbPath)
    {
        _dbOperator = new EdDatabaseOperator(dbPath, true);
        _multipleKey = new InitialMultipleKeyExchanger();
        _edCryptor = new EdDataCryptor(MultipleEncryptionCount);
        _hashCalculator = new RandomizedHashCalculator(new byte[64], MultipleKey.HashSeed);
        StashInitialMultipleKeyIfNotExists();
    }
    protected override byte[] GenerateIndexBytes(string name)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        byte[] rawIndexBytes = nameBytes;
        return _hashCalculator.ComputeHash(rawIndexBytes, HashStretchingCount);
    }
    private void StashInitialMultipleKeyIfNotExists()
    {
        if(IsIndexExists(InitialMultipleKeyIndexName)) return;
        var stashedMultipleKey = new StashedMultipleKeyExchanger();
        Stash(InitialMultipleKeyIndexName, stashedMultipleKey.GetBytes());
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
