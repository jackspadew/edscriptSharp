namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataCryptor
{
    byte[] EncryptBytes(byte[] dataBytes, IMultipleKeyExchanger multiKey);
    byte[] DecryptBytes(byte[] dataBytes, IMultipleKeyExchanger multiKey);
    Stream EncryptStream(Stream dataStream, IMultipleKeyExchanger multiKey);
    Stream DecryptStream(Stream dataStream, IMultipleKeyExchanger multiKey);
}
