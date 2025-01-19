namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataWorker
{
    void Stash(string index, byte[] data);
    byte[] Extract(string index);
}

public interface IEdDataWorkerChain
{
    int Depth {get;}
}
