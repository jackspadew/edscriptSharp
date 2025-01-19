namespace LibEncryptedDriveScripts.EdData;

public abstract class EdDataWorkerChainBase : EdDataWorkerBase, IEdDataWorker, IEdDataWorkerChain
{
    private int _depth;
    public int Depth { get => _depth; }
    protected IEdDataWorker _parentWorker;

    protected EdDataWorkerChainBase(IEdDataLogicFactory logicFactory, IEdDataWorker parentWorker) : base(logicFactory)
    {
        _parentWorker = parentWorker;
        if(_parentWorker is IEdDataWorkerChain worker)
        {
            _depth = worker.Depth + 1;
        }
    }
    public void Stash(string index, byte[] data)
    {
        throw new NotImplementedException();
    }
    public byte[] Extract(string index)
    {
        throw new NotImplementedException();
    }
}
