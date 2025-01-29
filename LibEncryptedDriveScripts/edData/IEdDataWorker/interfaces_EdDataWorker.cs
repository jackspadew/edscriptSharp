namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataWorker
{
    void Stash(string index, byte[] data);
    byte[] Extract(string index);
}

public interface IEdDataWorkerChain : IEdDataWorker
{
    int Depth {get;}
    void StashChildMultipleKey(string index);
    void RegenerateChildMultipleKey(string index);
    IMultipleKeyExchanger ExtractChildMultipleKey(string index);
}

public interface IEdDataWorkerInitializer : IEdDataWorker
{
    IMultipleKeyExchanger ExtractInitialMultipleKey();
}
