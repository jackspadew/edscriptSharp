namespace LibEd.EdData;

using LibEd.Database;

public abstract class EdDataLogicFactoryBase : IEdDataLogicFactory
{
    public static int MultipleEncryptionCount = 1000;
    protected abstract string DbPath {get;set;}
    protected int TargetWorkerChainDepth = 1;
    protected abstract byte[] Key {get; set;}

    private T DetermineObjectByWorkerType<T>(IEdDataWorker context, T defaultObject, T forInitializer)
    {
        return DetermineObjectByWorkerType(context, defaultObject, forInitializer, defaultObject, defaultObject, defaultObject);
    }
    private T DetermineObjectByWorkerType<T>(IEdDataWorker context, T defaultObject, T forInitializer, T forChain)
    {
        return DetermineObjectByWorkerType(context, defaultObject, forInitializer, forChain, forChain, forChain);
    }
    protected T DetermineObjectByWorkerType<T>(IEdDataWorker context, T defaultObject, T forInitializer, T forChainZero, T forChain, T forLastChain)
    {
        if(context is IEdDataWorkerInitializer)
        {
            return forInitializer;
        }
        else if(context is IEdDataWorkerChain chainWorker)
        {
            if(chainWorker.Depth == 0) return forChainZero;
            if(chainWorker.Depth == TargetWorkerChainDepth) return forLastChain;
            return forChain;
        }
        return defaultObject;
    }
    public virtual IEdDataCryptor CreateCryptor(IEdDataWorker thisInstance)
    {
        return DetermineObjectByWorkerType(
            thisInstance,
            DefaultCryptor,
            InitialCryptor);
    }
    protected abstract IEdDataCryptor InitialCryptor {get;}
    protected abstract IEdDataCryptor DefaultCryptor {get;}
    public virtual IDatabaseOperator CreateDatabaseOperator(IEdDataWorker thisInstance)
    {
        return DetermineObjectByWorkerType(
            thisInstance,
            DefaultDatabaseOperator,
            DefaultDatabaseOperator,
            DefaultDatabaseOperator,
            DefaultDatabaseOperator,
            LastWorkerDatabaseOperator);
    }
    protected abstract IDatabaseOperator DefaultDatabaseOperator {get;}
    protected abstract IDatabaseOperator LastWorkerDatabaseOperator {get;}
    public virtual IEdDataHashCalculator CreateHashCalculator(IEdDataWorker thisInstance)
    {
        return DefaultHashCalculator;
    }
    protected abstract IEdDataHashCalculator DefaultHashCalculator {get;}
    public virtual IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker thisInstance)
    {
        return DetermineObjectByWorkerType(
            thisInstance,
            GeneralMultipleKeyExchanger,
            DefaultMultipleKeyExchanger,
            ChainedMultipleKeyExchanger,
            ChainedMultipleKeyExchanger,
            ChainedMultipleKeyExchanger);
    }
    public IMultipleKeyExchanger CreateKeyBlendedMultipleKeyExchanger(IEdDataWorker thisInstance)
    {
        IMultipleKeyExchanger keyBlendedMultiKey = KeyBlendedMultipleKeyExchanger;
        keyBlendedMultiKey.Key = Key;
        return keyBlendedMultiKey;
    }
    protected abstract IMultipleKeyExchanger DefaultMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger KeyBlendedMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger ChainedMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger GeneralMultipleKeyExchanger {get;}
    public virtual IEdDataWorker CreateWorker()
    {
        var initialWorker = CreateInitialWorker();
        IEdDataWorker leafWorker = initialWorker;
        for(int currentDepth=0; currentDepth <= TargetWorkerChainDepth; currentDepth++)
        {
            leafWorker = CreateChainWorker(leafWorker);
        }
        return leafWorker;
    }
    protected abstract IEdDataWorkerInitializer CreateInitialWorker();
    protected abstract IEdDataWorkerChain CreateChainWorker(IEdDataWorker parentWorker);
    protected virtual IMultipleKeyExchanger MultipleKeyExchangerForHashPassword => GeneralMultipleKeyExchanger;
    protected virtual IEdDataHashCalculator HashCalculatorForHashPassword => DefaultHashCalculator;
    public virtual void SetPassword(string password)
    {
        int keyLength = KeyBlendedMultipleKeyExchanger.Key.Length;
        this.Key = new byte[keyLength];
        var converter = new StringToHashConverter(HashCalculatorForHashPassword, MultipleKeyExchangerForHashPassword);
        byte[] hash = converter.Convert(password);
        Array.Copy(hash, 0, this.Key, 0, this.Key.Length);
    }
}
