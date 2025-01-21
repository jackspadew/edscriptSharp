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
    public virtual void StashChildMultipleKey(string index)
    {
        if(_parentWorker is IEdDataWorkerChain parentChainWorker)
        {
            parentChainWorker.StashChildMultipleKey(index);
        }
        ExtractOwnMultipleKey(index);
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        childMultiKey.Randomize();
        byte[] childMultiKeyBytes = childMultiKey.GetBytes();
        base.Stash(index, childMultiKeyBytes);
    }
    public virtual IMultipleKeyExchanger ExtractChildMultipleKey(string index)
    {
        ExtractOwnMultipleKey(index);
        byte[] childMultipleKeyBytes = base.Extract(index);
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        childMultiKey.SetBytes(childMultipleKeyBytes);
        return childMultiKey;
    }
    protected virtual void ExtractOwnMultipleKey(string index)
    {
        if(_parentWorker is IEdDataWorkerChain chainworker)
        {
            var myChildMultiKey = chainworker.ExtractChildMultipleKey(index);
            _multipleKey = myChildMultiKey;
        }
        else if(_parentWorker is IEdDataWorkerInitializer initializer)
        {
            var initialMultiKey = initializer.ExtractInitialMultipleKey();
            var keyBlendedMultiKey = _logicFactory.CreateKeyBlendedMultipleKeyExchanger(this);
            initialMultiKey.CopyTo(keyBlendedMultiKey);
            _multipleKey = initialMultiKey;
        }
    }
}
