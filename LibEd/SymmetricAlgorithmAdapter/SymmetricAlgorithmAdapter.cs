namespace LibEd.SymmetricAlgorithmAdapter;

public abstract class SymmetricAlgorithmAdapterBase : ISymmetricAlgorithmAdapter
{
    public virtual int LegalIVSize => throw new NotImplementedException();
    public virtual int LegalKeySize => throw new NotImplementedException();

    public abstract void Decrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv);
    public virtual byte[] DecryptBytes(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        byte[] result;
        using (MemoryStream encryptedDataStream = new MemoryStream(encryptedBytes))
        using (MemoryStream decryptedStream = new MemoryStream())
        {
            Decrypt(encryptedDataStream, decryptedStream, key, iv);
            result = decryptedStream.ToArray();
        }
        return result;
    }
    public abstract void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv);
    public virtual byte[] EncryptBytes(byte[] inputBytes, byte[] key, byte[] iv)
    {
        byte[] result;
        using (MemoryStream inputStream = new MemoryStream(inputBytes))
        using (MemoryStream encryptedStream = new MemoryStream())
        {
            Encrypt(inputStream, encryptedStream, key, iv);
            result = encryptedStream.ToArray();
        }
        return result;
    }
    public abstract Stream CreateWritableDecryptStream(Stream outputStream, byte[] key, byte[] iv);
    public abstract Stream CreateWritableEncryptStream(Stream outputStream, byte[] key, byte[] iv);
}
