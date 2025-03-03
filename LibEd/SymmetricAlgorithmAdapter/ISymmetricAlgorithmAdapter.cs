namespace LibEd.SymmetricAlgorithmAdapter;

public interface ISymmetricAlgorithmAdapter
{
    int LegalIVSize { get; }
    int LegalKeySize { get; }
    void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv);
    void Decrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv);
    byte[] EncryptBytes(byte[] inputBytes, byte[] key, byte[] iv);
    byte[] DecryptBytes(byte[] inputBytes, byte[] key, byte[] iv);
    Stream CreateWritableEncryptStream(Stream outputStream, byte[] key, byte[] iv);
    Stream CreateWritableDecryptStream(Stream outputStream, byte[] key, byte[] iv);
}
