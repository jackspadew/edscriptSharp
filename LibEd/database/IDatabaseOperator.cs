namespace LibEd.Database;

public interface IDatabaseOperator
{
    void InsertData(byte[] index, byte[] data);
    void InsertData(byte[] index, Stream readableStream);
    void UpdateData(byte[] index, byte[] data);
    void UpdateData(byte[] index, Stream readableStream);
    Stream GetDataStream(byte[] index);
    byte[] GetDataBytes(byte[] index);
    bool IsIndexExists(byte[] index);
}
