namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataWorker
{
    void Stash(string index, byte[] data);
    byte[] Extract(string index);
    void SetMultipleKey(IMultipleKeyExchanger multiKey);
    void SetSecretKey(byte[] key);
}

public interface IEdDataWorkerFactory
{
    IEdDataWorker NextWorker();
}
