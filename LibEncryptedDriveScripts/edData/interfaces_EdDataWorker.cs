namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataWorker
{
    void Stash(string index, byte[] data);
    byte[] Extract(string index);
}

public interface IEdDataWorkerChain
{
    int Depth {get;}
    void StashChildMultipleKey(string index);
    IMultipleKeyExchanger ExtractChildMultipleKey(string index);
}

public interface IEdDataWorkerInitializer
{
    IMultipleKeyExchanger ExtractInitialMultipleKey();
}
