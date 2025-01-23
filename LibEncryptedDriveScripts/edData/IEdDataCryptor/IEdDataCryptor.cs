namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataCryptor
{
    byte[] EncryptBytes(byte[] dataBytes, IMultipleKeyExchanger multiKey);
    byte[] DecryptBytes(byte[] dataBytes, IMultipleKeyExchanger multiKey);
    void EncryptStream(Stream dataStream, Stream outputStream, IMultipleKeyExchanger multiKey);
    void DecryptStream(Stream dataStream, Stream outputStream, IMultipleKeyExchanger multiKey);
}
