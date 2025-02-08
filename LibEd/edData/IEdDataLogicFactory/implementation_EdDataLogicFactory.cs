using LibEd.Database;

namespace LibEd.EdData;

public class BasicEdDataLogicFactory : EdDataLogicFactoryBase, IEdDataLogicFactory
{
    protected override string DbPath {get;set;}
    protected override byte[] Key {get;set;} = new byte[0];

    public BasicEdDataLogicFactory(string dbPath, string password) : this(dbPath, password, 1)
    {}
    public BasicEdDataLogicFactory(string dbPath, string password, int depth)
    {
        TargetWorkerChainDepth = depth;
        DbPath = dbPath;
        Key = new byte[KeyBlendedMultipleKeyExchanger.Key.Length];
        SetPassword(password);
    }

    protected override IEdDataCryptor DefaultCryptor => new EdDataCryptor(1000);
    protected override IEdDataCryptor InitialCryptor => new EdDataCryptor(10);
    protected override IEdDataCryptor ChainZeroCryptor => new EdDataCryptor(10000);
    protected override IEdDataCryptor MiddleWorkerCryptor => new EdDataCryptor(1000);
    protected override IEdDataCryptor LastWorkerCryptor => new EdDataCryptor(300);
    protected override IDatabaseOperator DefaultDatabaseOperator => new FakeInsertionDatabaseOperator(DbPath, true, 9999);
    protected override IDatabaseOperator LastWorkerDatabaseOperator => new EdDatabaseOperator(DbPath, true);
    protected override IEdDataHashCalculator DefaultHashCalculator => new EdDataHashCalculator(10000);
   protected override IEdDataHashCalculator InitialWorkerHashCalculator => new EdDataHashCalculator(10);
    protected override IEdDataHashCalculator ChainZeroWorkerHashCalculator => new EdDataHashCalculator(10000);
    protected override IEdDataHashCalculator MiddleChainWorkerHashCalculator => new EdDataHashCalculator(5000);
    protected override IEdDataHashCalculator LastWorkerHashCalculator => new EdDataHashCalculator(5000);
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
