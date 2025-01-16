namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataWorker
{
    void Stash(string index, byte[] data);
    byte[] Extract(string index);
    void SetMultipleKey(IMultipleKeyExchanger multiKey);
}
