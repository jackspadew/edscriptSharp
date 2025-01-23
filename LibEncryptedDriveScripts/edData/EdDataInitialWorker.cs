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
        var initialMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        initialMultiKey.Randomize();
        Stash(InitialMultipleKeyIndexName, initialMultiKey.GetBytes());
    }
    public IMultipleKeyExchanger ExtractInitialMultipleKey()
    {
        byte[] initialMultiKeyBytes = Extract(InitialMultipleKeyIndexName);
        var initialMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        initialMultiKey.SetBytes(initialMultiKeyBytes);
        return initialMultiKey;
    }
}
