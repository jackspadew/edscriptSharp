using LibEd.Database;

namespace LibEd.EdData;

public class BasicEdDataLogicFactory : EdDataLogicFactoryBase, IEdDataLogicFactory
{
    private const int MULTIPLE_ENCRYPT_COUNT_HEAVY = 1000;
    private const int MULTIPLE_ENCRYPT_COUNT_NORMAL = 300;
    private const int MULTIPLE_ENCRYPT_COUNT_LIGTH = 10;
    private const int DBOPERATOR_FAKEINSERTION_COUNT_HEAVY = 998;
    private const int DBOPERATOR_FAKEINSERTION_COUNT_NORMAL = 98;
    private const int DBOPERATOR_FAKEINSERTION_COUNT_LIGTH = 10;
    private const int HASH_STRETCHING_COUNT_HEAVY = 100000;
    private const int HASH_STRETCHING_COUNT_NORMAL = 10000;
    private const int HASH_STRETCHING_COUNT_LIGTH = 10;
    private const int WORKERCHAIN_DEPTH_LIGTH = 1;
    private const int WORKERCHAIN_DEPTH_HEAVY = 3;
    protected int Default_Multiple_Encrypt_Count = MULTIPLE_ENCRYPT_COUNT_HEAVY;
    protected int InitialWorker_Multiple_Encrypt_Count = MULTIPLE_ENCRYPT_COUNT_LIGTH;
    protected int ChainZeroWorker_Multiple_Encrypt_Count = MULTIPLE_ENCRYPT_COUNT_HEAVY;
    protected int MiddleWorker_Multiple_Encrypt_Count = MULTIPLE_ENCRYPT_COUNT_HEAVY;
    protected int LastWorker_Multiple_Encrypt_Count = MULTIPLE_ENCRYPT_COUNT_NORMAL;
    protected int Default_FakeInsertionCount = DBOPERATOR_FAKEINSERTION_COUNT_HEAVY;
    protected int Default_Hash_Stretching_Count = HASH_STRETCHING_COUNT_HEAVY;
    protected int InitialWorker_Hash_Stretching_Count = HASH_STRETCHING_COUNT_LIGTH;
    protected int ChainZeroWorker_Hash_Stretching_Count = HASH_STRETCHING_COUNT_HEAVY;
    protected int MiddleWorker_Hash_Stretching_Count = HASH_STRETCHING_COUNT_HEAVY;
    protected int LastWorker_Hash_Stretching_Count = HASH_STRETCHING_COUNT_HEAVY;
    protected override string DbPath {get;set;}
    protected override byte[] Key {get;set;} = new byte[0];

    public BasicEdDataLogicFactory(
        string dbPath,
        string password,
        int default_MultipleEncryptCount = MULTIPLE_ENCRYPT_COUNT_HEAVY,
        int initialWorker_MultipleEncryptCount = MULTIPLE_ENCRYPT_COUNT_LIGTH,
        int chainZeroWorker_MultipleEncryptCount = MULTIPLE_ENCRYPT_COUNT_HEAVY,
        int middleWorker_MultipleEncryptCount = MULTIPLE_ENCRYPT_COUNT_HEAVY,
        int lastWorker_MultipleEncryptCount = MULTIPLE_ENCRYPT_COUNT_NORMAL,
        int default_FakeInsertionCount = DBOPERATOR_FAKEINSERTION_COUNT_HEAVY,
        int default_HashStretchingCount = HASH_STRETCHING_COUNT_HEAVY,
        int initialWorker_HashStretchingCount = HASH_STRETCHING_COUNT_LIGTH,
        int chainZeroWorker_HashStretchingCount = HASH_STRETCHING_COUNT_HEAVY,
        int middleWorker_HashStretchingCount = HASH_STRETCHING_COUNT_HEAVY,
        int lastWorker_HashStretchingCount = HASH_STRETCHING_COUNT_HEAVY,
        int workerChainDepth = WORKERCHAIN_DEPTH_LIGTH
        ) : this(dbPath, password)
    {
        this.Default_Multiple_Encrypt_Count = default_MultipleEncryptCount;
        this.InitialWorker_Multiple_Encrypt_Count = initialWorker_MultipleEncryptCount;
        this.ChainZeroWorker_Multiple_Encrypt_Count = chainZeroWorker_MultipleEncryptCount;
        this.MiddleWorker_Multiple_Encrypt_Count = middleWorker_MultipleEncryptCount;
        this.LastWorker_Multiple_Encrypt_Count = lastWorker_MultipleEncryptCount;
        this.Default_FakeInsertionCount = default_FakeInsertionCount;
        this.Default_Hash_Stretching_Count = default_HashStretchingCount;
        this.InitialWorker_Hash_Stretching_Count = initialWorker_HashStretchingCount;
        this.ChainZeroWorker_Hash_Stretching_Count = chainZeroWorker_HashStretchingCount;
        this.MiddleWorker_Hash_Stretching_Count = middleWorker_HashStretchingCount;
        this.LastWorker_Hash_Stretching_Count = lastWorker_HashStretchingCount;
        this.TargetWorkerChainDepth = workerChainDepth;
    }
    public BasicEdDataLogicFactory(string dbPath, string password)
    {
        TargetWorkerChainDepth = WORKERCHAIN_DEPTH_LIGTH;
        DbPath = dbPath;
        Key = new byte[KeyBlendedMultipleKeyExchanger.Key.Length];
        SetPassword(password);
    }

    protected override IEdDataCryptor DefaultCryptor => new EdDataCryptor(Default_Multiple_Encrypt_Count);
    protected override IEdDataCryptor InitialCryptor => new EdDataCryptor(InitialWorker_Multiple_Encrypt_Count);
    protected override IEdDataCryptor ChainZeroCryptor => new EdDataCryptor(ChainZeroWorker_Multiple_Encrypt_Count);
    protected override IEdDataCryptor MiddleWorkerCryptor => new EdDataCryptor(MiddleWorker_Multiple_Encrypt_Count);
    protected override IEdDataCryptor LastWorkerCryptor => new EdDataCryptor(LastWorker_Multiple_Encrypt_Count);
    protected override IDatabaseOperator DefaultDatabaseOperator => new FakeInsertionDatabaseOperator(DbPath, true, Default_FakeInsertionCount);
    protected override IDatabaseOperator LastWorkerDatabaseOperator => new EdDatabaseOperator(DbPath, true);
    protected override IEdDataHashCalculator DefaultHashCalculator => new EdDataHashCalculator(Default_Hash_Stretching_Count);
    protected override IEdDataHashCalculator InitialWorkerHashCalculator => new EdDataHashCalculator(InitialWorker_Hash_Stretching_Count);
    protected override IEdDataHashCalculator ChainZeroWorkerHashCalculator => new EdDataHashCalculator(ChainZeroWorker_Hash_Stretching_Count);
    protected override IEdDataHashCalculator MiddleChainWorkerHashCalculator => new EdDataHashCalculator(MiddleWorker_Hash_Stretching_Count);
    protected override IEdDataHashCalculator LastWorkerHashCalculator => new EdDataHashCalculator(LastWorker_Hash_Stretching_Count);
    protected override IMultipleKeyExchanger DefaultMultipleKeyExchanger => new DefaultMultipleKeyExchanger();
    protected override IMultipleKeyExchanger KeyBlendedMultipleKeyExchanger => new BasicKeyBlendedMultipleKeyExchanger();
    protected override IMultipleKeyExchanger ChainedMultipleKeyExchanger => new BasicExemplaryMultipleKeyExchanger();
    protected override IMultipleKeyExchanger GeneralMultipleKeyExchanger => new BasicExemplaryMultipleKeyExchanger();
    protected override IEdDataWorkerChain CreateChainWorker(IEdDataWorker parentWorker)
    {
        return new EdDataWorkerChain(this, parentWorker);
    }
    protected override IEdDataWorkerInitializer CreateInitialWorker()
    {
        return new EdDataInitialWorker(this);
    }
}
