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
        ExtractOwnMultipleKey(index);
        base.Stash(index, data);
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
            return;
        }
        else if(_parentWorker is IEdDataWorkerInitializer initializer)
        {
            var initialMultiKey = initializer.ExtractInitialMultipleKey();
            var keyBlendedMultiKey = _logicFactory.CreateKeyBlendedMultipleKeyExchanger(this);
            byte[] key = (byte[])keyBlendedMultiKey.Key.Clone();
            initialMultiKey.CopyTo(keyBlendedMultiKey);
            keyBlendedMultiKey.Key = key;
            _multipleKey = keyBlendedMultiKey;
            return;
        }
        throw new InvalidOperationException("Can not extract own multiple key. The parent worker dotes not have valid interface.");
    }
}
