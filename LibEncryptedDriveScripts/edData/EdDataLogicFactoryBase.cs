namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public abstract class EdDataLogicFactoryBase : IEdDataLogicFactory
{
    public static int MultipleEncryptionCount = 1000;
    protected abstract string DbPath {get;set;}
    protected int TargetWorkerChainDepth = 1;

    private T DetermineObjectByWorkerType<T>(IEdDataWorker context, T defaultObject, T forInitializer)
    {
        return DetermineObjectByWorkerType(context, defaultObject, forInitializer, defaultObject, defaultObject);
    }
    private T DetermineObjectByWorkerType<T>(IEdDataWorker context, T defaultObject, T forInitializer, T forChain)
    {
        return DetermineObjectByWorkerType(context, defaultObject, forInitializer, forChain, forChain);
    }
    protected T DetermineObjectByWorkerType<T>(IEdDataWorker context, T defaultObject, T forInitializer, T forChainZero, T forChain)
    {
        if(context is IEdDataWorkerInitializer)
        {
            return forInitializer;
        }
        else if(context is IEdDataWorkerChain chainWorker)
        {
            if(chainWorker.Depth == 0) return forChainZero;
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
        return DefaultDatabaseOperator;
    }
    protected abstract IDatabaseOperator DefaultDatabaseOperator {get;}
    public virtual IEdDataHashCalculator CreateHashCalculator(IEdDataWorker thisInstance)
    {
        return DefaultHashCalculator;
    }
    protected abstract IEdDataHashCalculator DefaultHashCalculator {get;}
    public virtual IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker thisInstance)
    {
        return DetermineObjectByWorkerType(
            thisInstance,
            DefaultMultipleKeyExchanger,
            InitialMultipleKeyExchanger,
            SecretKeyCombinedMultipleKeyExchanger,
            ChainedMultipleKeyExchanger);
    }
    protected abstract IMultipleKeyExchanger InitialMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger SecretKeyCombinedMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger ChainedMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger DefaultMultipleKeyExchanger {get;}
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
}
