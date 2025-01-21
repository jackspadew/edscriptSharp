namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;
using LibEncryptedDriveScripts.HashCalculator;
using System.Text;

public class EdDataInitialWorker : EdDataWorkerBase, IEdDataWorker, IEdDataWorkerInitializer
{
    private readonly string InitialMultipleKeyIndexName = "__InitialMultiKey";
    protected override IMultipleKeyExchanger MultipleKey => _logicFactory.CreateMultipleKeyExchanger(this);

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
