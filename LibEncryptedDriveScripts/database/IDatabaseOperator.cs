namespace LibEncryptedDriveScripts.Database;

public interface IDatabaseOperator
{
    void InsertData(byte[] index, byte[] data);
    void InsertData(byte[] index, Stream readableStream);
    void UpdateData(byte[] index, byte[] data);
    Stream GetDataStream(byte[] index);
    byte[] GetDataBytes(byte[] index);
    bool IsIndexExists(byte[] index);
}
