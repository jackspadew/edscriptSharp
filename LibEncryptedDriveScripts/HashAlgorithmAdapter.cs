namespace LibEncryptedDriveScripts.HashAlgorithmAdapter;

using LibEncryptedDriveScripts.SaltStream;

public abstract class HashAlgorithmAdapterBase : IHashAlgorithmAdapter
{
    public abstract byte[] ComputeHash(byte[] inputBytes);
    public abstract byte[] ComputeHash(Stream inputStream);
    public virtual byte[] ComputeHash(byte[] inputBytes, byte[] salt)
    {
        byte[] saltedBytes = ToSalted(inputBytes, salt);
        return ComputeHash(saltedBytes);
    }
    public virtual byte[] ComputeHash(Stream inputStream, byte[] salt)
    {
        Stream saltedStream = ToSalted(inputStream, salt);
        return ComputeHash(saltedStream);
    }
    protected virtual byte[] ToSalted(byte[] source, byte[] salt)
    {
        return source.Concat(salt).ToArray();
    }
    protected virtual Stream ToSalted(Stream source, byte[] salt)
    {
        Stream saltedStream = new SaltStream(source, salt, true);
        return saltedStream;
    }
}
