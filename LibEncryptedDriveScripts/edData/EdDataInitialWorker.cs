namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.HashCalculator;
using System.Text;

public class EdDataInitialWorker : EdDataWorkerBase, IEdDataWorker, IEdDataWorkerInitializer
{
    private readonly string InitialMultipleKeyIndexName = "__InitialMultiKey";

    public EdDataInitialWorker(IEdDataLogicFactory logicFactory) : base(logicFactory)
    {
        StashInitialMultipleKeyIfNotExists();
    }
    private void StashInitialMultipleKeyIfNotExists()
    {
        if(IsIndexExists(InitialMultipleKeyIndexName)) return;
        var stashedMultipleKey = _logicFactory.CreateMultipleKeyExchanger(this);
        Stash(InitialMultipleKeyIndexName, stashedMultipleKey.GetBytes());
    }
    public IMultipleKeyExchanger ExtractInitialMultipleKey()
    {
        byte[] initMultiKeyBytes = Extract(InitialMultipleKeyIndexName);
        var initMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        initMultiKey.SetBytes(initMultiKeyBytes);
        return initMultiKey;
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
