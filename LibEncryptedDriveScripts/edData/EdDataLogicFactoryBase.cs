namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public abstract class EdDataLogicFactoryBase : IEdDataLogicFactory
{
    public static int MultipleEncryptionCount = 1000;
    protected abstract string DbPath {get;set;}

    private T DetermineObjectByWorkerType<T>(IEdDataWorker worker, T defaultObject, T forInitializer)
    {
        return DetermineObjectByWorkerType(worker, defaultObject, forInitializer, defaultObject, defaultObject);
    }
    private T DetermineObjectByWorkerType<T>(IEdDataWorker worker, T defaultObject, T forInitializer, T forChain)
    {
        return DetermineObjectByWorkerType(worker, defaultObject, forInitializer, forChain, forChain);
    }
    protected T DetermineObjectByWorkerType<T>(IEdDataWorker worker, T defaultObject, T forInitializer, T forChainZero, T forChain)
    {
        if(worker is IEdDataWorkerInitializer)
        {
            return forInitializer;
        }
        else if(worker is IEdDataWorkerChain chainWorker)
        {
            if(chainWorker.Depth == 0) return forChainZero;
            return forChain;
        }
        return defaultObject;
    }
    public virtual IEdDataCryptor CreateCryptor(IEdDataWorker worker)
    {
        return DetermineObjectByWorkerType(
            worker,
            DefaultCryptor,
            InitialCryptor);
    }
    protected abstract IEdDataCryptor InitialCryptor {get;}
    protected abstract IEdDataCryptor DefaultCryptor {get;}
    public virtual IDatabaseOperator CreateDatabaseOperator(IEdDataWorker worker)
    {
        return DefaultDatabaseOperator;
    }
    protected abstract IDatabaseOperator DefaultDatabaseOperator {get;}
    public virtual IEdDataHashCalculator CreateHashCalculator(IEdDataWorker worker)
    {
        return DefaultHashCalculator;
    }
    protected abstract IEdDataHashCalculator DefaultHashCalculator {get;}
    public virtual IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker worker)
    {
        return DetermineObjectByWorkerType(
            worker,
            DefaultMultipleKeyExchanger,
            InitialMultipleKeyExchanger,
            SecretKeyCombinedMultipleKeyExchanger,
            ChainedMultipleKeyExchanger);
    }
    protected abstract IMultipleKeyExchanger InitialMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger SecretKeyCombinedMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger ChainedMultipleKeyExchanger {get;}
    protected abstract IMultipleKeyExchanger DefaultMultipleKeyExchanger {get;}
    public virtual IEdDataWorker CreateWorker(IEdDataWorker worker)
    {
        throw new NotImplementedException();
    }
}
