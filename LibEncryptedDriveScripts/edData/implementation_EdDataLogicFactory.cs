using LibEncryptedDriveScripts.Database;

namespace LibEncryptedDriveScripts.EdData;

public class BasicEdDataLogicFactory : EdDataLogicFactoryBase, IEdDataLogicFactory
{
    protected override string DbPath {get;set;}
    protected override byte[] Key {get;set;} = new byte[0];

    public BasicEdDataLogicFactory(string dbPath, string password)
    {
        DbPath = dbPath;
        SetPassword(password);
    }

    protected override IEdDataCryptor InitialCryptor => new EdDataCryptor(10);
    protected override IEdDataCryptor DefaultCryptor => new EdDataCryptor(1000);
    protected override IDatabaseOperator DefaultDatabaseOperator => new EdDatabaseOperator(DbPath, true);
    protected override IEdDataHashCalculator DefaultHashCalculator => new EdDataHashCalculator();
    protected override IMultipleKeyExchanger InitialMultipleKeyExchanger => new InitialMultipleKeyExchanger();
    protected override IMultipleKeyExchanger KeyBlendedMultipleKeyExchanger => new BasicKeyBlendedMultipleKeyExchanger();
    protected override IMultipleKeyExchanger ChainedMultipleKeyExchanger => new BasicExemplaryMultipleKeyExchanger();
    protected override IMultipleKeyExchanger DefaultMultipleKeyExchanger => new BasicExemplaryMultipleKeyExchanger();
    protected override IEdDataWorkerChain CreateChainWorker(IEdDataWorker parentWorker)
    {
        throw new NotImplementedException();
    }
    protected override IEdDataWorkerInitializer CreateInitialWorker()
    {
        throw new NotImplementedException();
    }
}
