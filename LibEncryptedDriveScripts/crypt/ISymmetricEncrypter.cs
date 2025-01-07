namespace LibEncryptedDriveScripts.SymmetricEncrypter;

public interface ISymmetricEncrypter
{
    byte[] Encrypt(byte[] inputBytes, byte[] key, byte[] iv);
    byte[] Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv);
    void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv);
    void Decrypt(Stream encryptedStream, Stream outputStream, byte[] key, byte[] iv);
}
