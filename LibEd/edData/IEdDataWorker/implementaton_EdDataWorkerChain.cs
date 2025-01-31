namespace LibEd.EdData;

public class EdDataWorkerChain : EdDataWorkerChainBase, IEdDataWorker, IEdDataWorkerChain
{
    public EdDataWorkerChain(IEdDataLogicFactory logicFactory, IEdDataWorker parentWorker) : base(logicFactory, parentWorker)
    {
    }
}
