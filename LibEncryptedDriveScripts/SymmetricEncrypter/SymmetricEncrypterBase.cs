namespace LibEd.SymmetricEncrypter;

using LibEd.SymmetricAlgorithmAdapter;

public abstract class SymmetricEncrypterBase : ISymmetricEncrypter
{
    protected abstract ISymmetricAlgorithmAdapter Algorithm {get;}

    public virtual byte[] Encrypt(byte[] inputBytes, byte[] key, byte[] iv)
    {
        return Algorithm.EncryptBytes(inputBytes, key, iv);
    }
    public virtual byte[] Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        return Algorithm.DecryptBytes(encryptedBytes, key, iv);
    }
    public virtual void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        Stream writableStream = Algorithm.CreateWritableEncryptStream(outputStream, key, iv);
        inputStream.CopyTo(writableStream);
        writableStream.Dispose();
    }
    public virtual void Decrypt(Stream encryptedStream, Stream outputStream, byte[] key, byte[] iv)
    {
        Stream writableStream = Algorithm.CreateWritableDecryptStream(outputStream, key, iv);
        encryptedStream.CopyTo(writableStream);
        writableStream.Dispose();
    }
}
