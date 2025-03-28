namespace LibEd.EdData;

public abstract class EdDataWorkerChainBase : EdDataWorkerBase, IEdDataWorker, IEdDataWorkerChain
{
    private int _depth;
    public int Depth { get => _depth; }
    protected IEdDataWorker _parentWorker;
    protected string? _lastExtractedMultipleKeyIndex;
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
        if(_parentWorker is IEdDataWorkerChain parentChainWorker)
        {
            _multipleKey = parentChainWorker.StashChildMultipleKey(index);
            _lastExtractedMultipleKeyIndex = index;
            ExtractOwnMultipleKey(index);
            if(IsIndexExists(index)) this.RegenerateOwnMultipleKey(index);
        }
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
    public virtual IMultipleKeyExchanger StashChildMultipleKey(string index)
    {
        if(_parentWorker is IEdDataWorkerChain parentChainWorker)
        {
            _multipleKey = parentChainWorker.StashChildMultipleKey(index);
            _lastExtractedMultipleKeyIndex = index;
            if(IsIndexExists(index)) this.RegenerateOwnMultipleKey(index);
        }
        else
        {
            ExtractOwnMultipleKey(index);
            if(IsIndexExists(index))
            {
                string IndexBytesString = BitConverter.ToString(GenerateIndexBytes(index));
                throw new ArgumentException($"The generated index collided. May be using the same INDEX string. If the INDEX string is different, this error may be resolved by reconstructing a database because it is based on a hash value collision. (index=\"{index})\" indexBytes=\"{IndexBytesString}\" this={nameof(this.GetType)} this._depth={_depth})");
            }
        }
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        childMultiKey.Randomize();
        byte[] childMultiKeyBytes = childMultiKey.GetBytes();
        try{
            base.Stash(index, childMultiKeyBytes);
        }
        catch (Exception ex)
        {
            throw new Exception($"{this.GetType().Name}: _depth={_depth}", ex);
        }
        return childMultiKey;
    }
    public virtual IMultipleKeyExchanger RegenerateChildMultipleKey(string index)
    {
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        childMultiKey.Randomize();
        this.UpdateStashedData(index, childMultiKey.GetBytes());
        return childMultiKey;
    }
    public virtual IMultipleKeyExchanger ExtractChildMultipleKey(string index)
    {
        ExtractOwnMultipleKey(index);
        byte[] childMultipleKeyBytes = base.Extract(index);
        var childMultiKey = _logicFactory.CreateMultipleKeyExchanger(this);
        childMultiKey.SetBytes(childMultipleKeyBytes);
        return childMultiKey;
    }
    protected virtual void ExtractOwnMultipleKey(string index, bool force=false)
    {
        if(_lastExtractedMultipleKeyIndex == index && !force) return;
        _lastExtractedMultipleKeyIndex = index;
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
            _multipleKey = parentChainWorker.RegenerateChildMultipleKey(index);
            _lastExtractedMultipleKeyIndex = index;
        }
    }
}
