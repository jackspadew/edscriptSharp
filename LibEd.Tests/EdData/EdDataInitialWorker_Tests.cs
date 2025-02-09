namespace LibEd.Tests;

using Xunit;
using Moq;
using LibEd.EdData;
using LibEd.Database;

public class EdDataInitialWorker_Tests
{
    public static string dbPath = "EdDataInitialWorker_Tests.db";

    public class Concrete_DefaultMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase {}
    public class Concrete_KeyBlendedMultipleKeyExchanger : BasicKeyBlendedMultipleKeyExchanger {}
    public class Concrete_LogicFactory : EdDataLogicFactoryBase, IEdDataLogicFactory
    {
        public Concrete_LogicFactory()
        {
            Key = new byte[KeyBlendedMultipleKeyExchanger.Key.Length];
        }
        protected override string DbPath { get => dbPath; set => throw new NotImplementedException(); }
        protected override byte[] Key { get; set; }

        protected override IEdDataCryptor DefaultCryptor => new EdDataCryptor();
        protected override IEdDataCryptor InitialCryptor => new EdDataCryptor();
        protected override IEdDataCryptor ChainZeroCryptor => new EdDataCryptor();
        protected override IEdDataCryptor MiddleWorkerCryptor => new EdDataCryptor();
        protected override IEdDataCryptor LastWorkerCryptor => new EdDataCryptor();
        protected override IDatabaseOperator DefaultDatabaseOperator => new EdDatabaseOperator(DbPath, true);
        protected override IDatabaseOperator LastWorkerDatabaseOperator => new EdDatabaseOperator(DbPath, true);
        protected override IEdDataHashCalculator DefaultHashCalculator => new EdDataHashCalculator();
        protected override IEdDataHashCalculator InitialWorkerHashCalculator => DefaultHashCalculator;
        protected override IEdDataHashCalculator ChainZeroWorkerHashCalculator => DefaultHashCalculator;
        protected override IEdDataHashCalculator MiddleChainWorkerHashCalculator => DefaultHashCalculator;
        protected override IEdDataHashCalculator LastWorkerHashCalculator => DefaultHashCalculator;
        protected override IMultipleKeyExchanger DefaultMultipleKeyExchanger => new Concrete_DefaultMultipleKeyExchanger();
        protected override IMultipleKeyExchanger KeyBlendedMultipleKeyExchanger => new Concrete_KeyBlendedMultipleKeyExchanger();
        protected override IMultipleKeyExchanger ChainedMultipleKeyExchanger => new Concrete_DefaultMultipleKeyExchanger();
        protected override IMultipleKeyExchanger GeneralMultipleKeyExchanger => new Concrete_DefaultMultipleKeyExchanger();
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

    [Fact]
    public void ExtractInitialMultipleKey_ReturnedMultipleKeyHasDifferentValuesFromInitialValue()
    {
        DeleteFileIfExists(dbPath);
        var logicFactory = CreateFactory();
        var initialWorker = new EdDataInitialWorker(logicFactory);
        var initialValuesMultipleKey = logicFactory.CreateMultipleKeyExchanger(initialWorker);
        var initialMultiKey = initialWorker.ExtractInitialMultipleKey();
        Assert.NotEqual(initialValuesMultipleKey.KeySeed, initialMultiKey.KeySeed);
        Assert.NotEqual(initialValuesMultipleKey.IVSeed, initialMultiKey.IVSeed);
        Assert.NotEqual(initialValuesMultipleKey.AlgorithmSeed, initialMultiKey.AlgorithmSeed);
        Assert.NotEqual(initialValuesMultipleKey.HashSeed, initialMultiKey.HashSeed);
        Assert.NotEqual(initialValuesMultipleKey.Key, initialMultiKey.Key);
        Assert.NotEqual(initialValuesMultipleKey.IV, initialMultiKey.IV);
        Assert.NotEqual(initialValuesMultipleKey.Salt, initialMultiKey.Salt);
        Assert.NotEqual(initialValuesMultipleKey.Lye, initialMultiKey.Lye);
    }
}
