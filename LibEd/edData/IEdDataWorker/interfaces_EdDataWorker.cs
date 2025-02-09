namespace LibEd.EdData;

public interface IEdDataWorker
{
    void Stash(string index, byte[] data);
    byte[] Extract(string index);
}

public interface IEdDataWorkerChain : IEdDataWorker
{
    int Depth {get;}
    IMultipleKeyExchanger StashChildMultipleKey(string index);
    IMultipleKeyExchanger RegenerateChildMultipleKey(string index);
    IMultipleKeyExchanger ExtractChildMultipleKey(string index);
}

public interface IEdDataWorkerInitializer : IEdDataWorker
{
    IMultipleKeyExchanger ExtractInitialMultipleKey();
}
