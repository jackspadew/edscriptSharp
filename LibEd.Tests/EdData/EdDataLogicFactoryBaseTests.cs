namespace LibEd.Tests;

using Xunit;
using LibEd.EdData;
using LibEd.Database;
using System.Text;

public class EdDataLogicFactoryBase_Tests
{
    private static byte[] exampleKey = new byte[32]{0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1};
    private static string examplePassword = "abcd";
    public class ConcreteCryptor_ForInitializer : EdDataCryptor {}
    public class ConcreteCryptor_Default : EdDataCryptor {}
    public class ConcreteDataOperator_Default : DatabaseOperatorBase
    {
        public ConcreteDataOperator_Default() : base("EdDataLogicFactoryBase_Tests.db", true)
        {}
    }
    public class ConcreteDataOperator_ForLastChain : DatabaseOperatorBase
    {
        public ConcreteDataOperator_ForLastChain() : base("EdDataLogicFactoryBase_Tests.db", true)
        {}
    }
    public class ConcreteHashCalculator_Default : EdDataHashCalculator {}
    public class ConcreteMultipleKeyExchanger_Default : ExemplaryMultipleKeyExchangerBase {}
    public class ConcreteMultipleKeyExchanger_ForInitializer: ExemplaryMultipleKeyExchangerBase {}
    public class ConcreteMultipleKeyExchanger_ForKeyBlending : ExemplaryMultipleKeyExchangerBase {}
    public class ConcreteMultipleKeyExchanger_ForChain : ExemplaryMultipleKeyExchangerBase {}
    public class Iplemented_EdDataLogicFactory : EdDataLogicFactoryBase
    {
        public int SetterForTest_TargetWorkerChainDepth {
            get {return base.TargetWorkerChainDepth;}
            set {base.TargetWorkerChainDepth = value;}
        }
        protected override string DbPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected override byte[] Key { get; set; }

        public Iplemented_EdDataLogicFactory() : this(examplePassword) {}
        public Iplemented_EdDataLogicFactory(string password)
        {
            Key = new byte[KeyBlendedMultipleKeyExchanger.Key.Length];
            SetPassword(password);
        }

        protected override IEdDataCryptor InitialCryptor => new ConcreteCryptor_ForInitializer();
        protected override IEdDataCryptor DefaultCryptor => new ConcreteCryptor_Default();
        protected override IDatabaseOperator DefaultDatabaseOperator => new ConcreteDataOperator_Default();
        protected override IDatabaseOperator LastWorkerDatabaseOperator => new ConcreteDataOperator_ForLastChain();
        protected override IEdDataHashCalculator DefaultHashCalculator => new ConcreteHashCalculator_Default();
        protected override IMultipleKeyExchanger InitialMultipleKeyExchanger => new ConcreteMultipleKeyExchanger_ForInitializer();
        protected override IMultipleKeyExchanger KeyBlendedMultipleKeyExchanger => new ConcreteMultipleKeyExchanger_ForKeyBlending();
        protected override IMultipleKeyExchanger ChainedMultipleKeyExchanger => new ConcreteMultipleKeyExchanger_ForChain();
        protected override IMultipleKeyExchanger GeneralMultipleKeyExchanger => new ConcreteMultipleKeyExchanger_Default();
        protected override IEdDataWorkerChain CreateChainWorker(IEdDataWorker parentWorker)
        {
            return new ConcreteChainWorker(this, parentWorker);
        }
        protected override IEdDataWorkerInitializer CreateInitialWorker()
        {
            return new ConcreteInitializer(this);
        }
    }

    public class ConcreteInitializer : EdDataInitialWorker
    {
        public ConcreteInitializer(IEdDataLogicFactory logicFactory) : base(logicFactory)
        {
        }
    }
    public class ConcreteChainWorker : EdDataWorkerChainBase
    {
        public ConcreteChainWorker(IEdDataLogicFactory logicFactory, IEdDataWorker parentWorker) : base(logicFactory, parentWorker)
        {
        }
    }

    [Fact]
    public void CreateObjectsWithInitialWorker_ReturnCorrectType()
    {
        var logicFactory = new Iplemented_EdDataLogicFactory();
        IEdDataWorker worker = new ConcreteInitializer(logicFactory);
        IEdDataCryptor cryptor = logicFactory.CreateCryptor(worker);
        IMultipleKeyExchanger multiKey = logicFactory.CreateMultipleKeyExchanger(worker);
        Assert.True( cryptor is ConcreteCryptor_ForInitializer );
        Assert.True( multiKey is ConcreteMultipleKeyExchanger_ForInitializer );
    }

    [Fact]
    public void CreateObjectsWithChainZeroWorker_ReturnCorrectType()
    {
        var logicFactory = new Iplemented_EdDataLogicFactory();
        IEdDataWorker initialWorker = new ConcreteInitializer(logicFactory);
        IEdDataWorker worker = new ConcreteChainWorker(logicFactory, initialWorker);
        IEdDataCryptor cryptor = logicFactory.CreateCryptor(worker);
        IMultipleKeyExchanger multiKey = logicFactory.CreateMultipleKeyExchanger(worker);
        Assert.True( cryptor is ConcreteCryptor_Default );
        Assert.True( multiKey is ConcreteMultipleKeyExchanger_ForChain );
    }

    [Fact]
    public void CreateObjectsWithMiddleWorker_ReturnCorrectType()
    {
        var logicFactory = new Iplemented_EdDataLogicFactory();
        logicFactory.SetterForTest_TargetWorkerChainDepth = 2;
        IEdDataWorker initialWorker = new ConcreteInitializer(logicFactory);
        IEdDataWorker chainzeroWorker = new ConcreteChainWorker(logicFactory, initialWorker);
        IEdDataWorker middleWorker = new ConcreteChainWorker(logicFactory, chainzeroWorker);
        IEdDataWorker lastWorker = new ConcreteChainWorker(logicFactory, middleWorker);
        IEdDataCryptor cryptor = logicFactory.CreateCryptor(middleWorker);
        IMultipleKeyExchanger multiKey = logicFactory.CreateMultipleKeyExchanger(middleWorker);
        IDatabaseOperator dbOperator = logicFactory.CreateDatabaseOperator(middleWorker);
        Assert.True( cryptor is ConcreteCryptor_Default );
        Assert.True( multiKey is ConcreteMultipleKeyExchanger_ForChain );
        Assert.True( dbOperator is ConcreteDataOperator_Default );
    }

    [Fact]
    public void CreateObjectsWithLastChainWorker_ReturnCorrectType()
    {
        var logicFactory = new Iplemented_EdDataLogicFactory();
        logicFactory.SetterForTest_TargetWorkerChainDepth = 2;
        IEdDataWorker initialWorker = new ConcreteInitializer(logicFactory);
        IEdDataWorker chainzeroWorker = new ConcreteChainWorker(logicFactory, initialWorker);
        IEdDataWorker middleWorker = new ConcreteChainWorker(logicFactory, chainzeroWorker);
        IEdDataWorker lastWorker = new ConcreteChainWorker(logicFactory, middleWorker);
        IEdDataCryptor cryptor = logicFactory.CreateCryptor(lastWorker);
        IMultipleKeyExchanger multiKey = logicFactory.CreateMultipleKeyExchanger(lastWorker);
        IDatabaseOperator dbOperator = logicFactory.CreateDatabaseOperator(lastWorker);
        Assert.True( cryptor is ConcreteCryptor_Default );
        Assert.True( multiKey is ConcreteMultipleKeyExchanger_ForChain );
        Assert.True( dbOperator is ConcreteDataOperator_ForLastChain );
    }

    [Fact]
    public void CreateWorker_ReturnedWorkerTypeIsCorrect()
    {
        var logicFactory = new Iplemented_EdDataLogicFactory();
        var worker = logicFactory.CreateWorker();
        Assert.True( worker is IEdDataWorkerChain );
    }

    [Fact]
    public void CreateWorker_ReturnedWorkerDepthIsCorrect()
    {
        var logicFactory = new Iplemented_EdDataLogicFactory();
        var worker = logicFactory.CreateWorker();
        if(worker is IEdDataWorkerChain chainWorker)
        {
            Assert.Equal(1, chainWorker.Depth);
            return;
        }
        Assert.Fail();
    }

    [Fact]
    public void CreateMultipleKeyExchangerForWorkerChainZero_TheKeyIsHashedBytes()
    {
        var hashCalculator = new ConcreteHashCalculator_Default();
        var passBytes = Encoding.UTF8.GetBytes(examplePassword);
        byte[] hash = hashCalculator.ComputeHash(passBytes, new ConcreteMultipleKeyExchanger_Default());
        var logicFactory = new Iplemented_EdDataLogicFactory();
        var initialWorker = new ConcreteInitializer(logicFactory);
        var workerChainZero = new ConcreteChainWorker(logicFactory,initialWorker);
        var multiKey = logicFactory.CreateKeyBlendedMultipleKeyExchanger(workerChainZero);
        Assert.Equal(hash[0..32].Length, multiKey.Key.Length);
        Assert.Equal(hash[0..32], multiKey.Key);
    }
}
