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
        try{
            base.Stash(index, data);
        }
        catch (Exception ex)
        {
            throw new Exception($"{this.GetType().Name}: _depth={_depth}", ex);
        }
    }
    public override byte[] Extract(string index)
    {
        ExtractOwnMultipleKey(index);
        return base.Extract(index);
    }
    protected virtual void UpdateStashedData(string index, byte[] data)
    {
        ExtractOwnMultipleKey(index);
        byte[] encryptedBytes = EdCryptor.EncryptBytes(data, MultipleKey);
        byte[] indexBytes = GenerateIndexBytes(index);
        try{
            DbOperator.UpdateData(indexBytes, encryptedBytes);
        }
        catch (Exception ex)
        {
            string IndexBytesString = BitConverter.ToString(GenerateIndexBytes(index));
            string OwnMultipleKeyString = MultipleKey.ToString();
            throw new Exception($"{this.GetType().Name}: IndexString={index}, IndexBytes={IndexBytesString}, MultipleKey={OwnMultipleKeyString}", ex);
        }
    }
    public virtual void StashChildMultipleKey(string index)
    {
        if(_parentWorker is IEdDataWorkerChain parentChainWorker)
        {
            parentChainWorker.StashChildMultipleKey(index);
        }
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        childMultiKey.Randomize();
        byte[] childMultiKeyBytes = childMultiKey.GetBytes();
        this.Stash(index, childMultiKeyBytes);
    }
    public virtual IMultipleKeyExchanger ExtractChildMultipleKey(string index)
    {
        ExtractOwnMultipleKey(index);
        if(!IsIndexExists(index))
        {
            StashChildMultipleKey(index);
        }
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
    protected virtual void RegenerateOwnMultipleKey(string index)
    {
        if(!(_parentWorker is IEdDataWorkerChain parentChainWorker))
        {
            throw new Exception($"If {nameof(_parentWorker)} is not {nameof(IEdDataWorkerChain)}, this method should not be called.");
        }
        while(IsIndexExists(index))
        {
            parentChainWorker.RegenerateChildMultipleKey(index);
        }
        ExtractOwnMultipleKey(index);
    }
}
