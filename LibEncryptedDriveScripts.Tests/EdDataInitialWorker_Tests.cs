namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using Moq;
using LibEncryptedDriveScripts.EdData;
using LibEncryptedDriveScripts.Database;

public class EdDataInitialWorker_Tests
{
    public static string dbPath = "EdDataInitialWorker_Tests.db";

    public class Concrete_InitialMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase {}
    public class Concrete_KeyBlendedMultipleKeyExchanger : KeyBlendedMultipleKeyExchangerBase {}
    public class Concrete_LogicFactory : EdDataLogicFactoryBase, IEdDataLogicFactory
    {
        public Concrete_LogicFactory()
        {
            Key = new byte[KeyBlendedMultipleKeyExchanger.Key.Length];
        }
        protected override string DbPath { get => dbPath; set => throw new NotImplementedException(); }
        protected override byte[] Key { get; set; }

        protected override IEdDataCryptor InitialCryptor => new EdDataCryptor();
        protected override IEdDataCryptor DefaultCryptor => new EdDataCryptor();
        protected override IDatabaseOperator DefaultDatabaseOperator => new EdDatabaseOperator(DbPath, true);
        protected override IEdDataHashCalculator DefaultHashCalculator => new EdDataHashCalculator();
        protected override IMultipleKeyExchanger InitialMultipleKeyExchanger => new Concrete_InitialMultipleKeyExchanger();
        protected override IMultipleKeyExchanger KeyBlendedMultipleKeyExchanger => new Concrete_KeyBlendedMultipleKeyExchanger();
        protected override IMultipleKeyExchanger ChainedMultipleKeyExchanger => new Concrete_InitialMultipleKeyExchanger();
        protected override IMultipleKeyExchanger DefaultMultipleKeyExchanger => new Concrete_InitialMultipleKeyExchanger();
        protected override IEdDataWorkerChain CreateChainWorker(IEdDataWorker parentWorker)
        {
            return new Concrete_ChainWorker(this, parentWorker);
        }
        protected override IEdDataWorkerInitializer CreateInitialWorker()
        {
            return new EdDataInitialWorker(this);
        }
    }

    public class Concrete_ChainWorker : EdDataWorkerChainBase
    {
        public Concrete_ChainWorker(IEdDataLogicFactory logicFactory, IEdDataWorker parentWorker) : base(logicFactory, parentWorker)
        {
        }
    }

    private IEdDataLogicFactory CreateFactory()
    {
        return new Concrete_LogicFactory();
    }

    private void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CreateObject_InitialMultiKeyIsExists()
    {
        DeleteFileIfExists(dbPath);
        var logicFactory = CreateFactory();
        var edWorker = new EdDataInitialWorker(logicFactory);
        bool IsInitialMultiKeyExists = edWorker.IsIndexExists("__InitialMultiKey");
        Assert.True(IsInitialMultiKeyExists);
    }
}
