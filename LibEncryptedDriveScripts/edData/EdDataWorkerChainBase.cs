namespace LibEncryptedDriveScripts.EdData;

public abstract class EdDataWorkerChainBase : EdDataWorkerBase, IEdDataWorker, IEdDataWorkerChain
{
    private int _depth;
    public int Depth { get => _depth; }
    protected IEdDataWorker _parentWorker;
    private IMultipleKeyExchanger? _multipleKey;
    protected override IMultipleKeyExchanger MultipleKey => _multipleKey ?? throw new NullReferenceException();

    protected EdDataWorkerChainBase(IEdDataLogicFactory logicFactory, IEdDataWorker parentWorker) : base(logicFactory)
    {
        _parentWorker = parentWorker;
        if(_parentWorker is IEdDataWorkerChain worker)
        {
            _depth = worker.Depth + 1;
        }
    }
    public override void Stash(string index, byte[] data)
    {
        _parentWorker.Stash(index, new byte[0]);
        ExtractOwnMultipleKey(index);
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger();
        childMultiKey.Randomize();
        byte[] childMultiKeyBytes = childMultiKey.GetBytes();
        base.Stash(index, childMultiKeyBytes);
    }
    public override byte[] Extract(string index)
    {
        ExtractOwnMultipleKey(index);
        return base.Extract(index);
    }
    protected virtual void ExtractOwnMultipleKey(string index)
    {
        byte[] myMultiKeyBytes = _parentWorker.Extract(index);
        var myMultiKey = _logicFactory.CreateMultipleKeyExchanger();
        myMultiKey.SetBytes(myMultiKeyBytes);
        _multipleKey = myMultiKey;
    }
}
