namespace LibEncryptedDriveScripts.Database;

public interface IDatabaseOperator
{
    void InsertData(byte[] index, byte[] data);
    Stream GetDataStream(byte[] index);
}
