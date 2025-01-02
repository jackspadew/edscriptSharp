namespace LibEncryptedDriveScripts.HashAlgorithmAdapter;

public abstract class HashAlgorithmAdapterBase : IHashAlgorithmAdapter
{
    public abstract byte[] ComputeHash(byte[] inputBytes);
    public abstract byte[] ComputeHash(Stream inputStream);
    public virtual byte[] ComputeHash(byte[] inputBytes, byte[] solt)
    {
        byte[] soltedBytes = ToSolted(inputBytes, solt);
        return ComputeHash(soltedBytes);
    }
    public virtual byte[] ComputeHash(Stream inputStream, byte[] solt)
    {
        Stream soltedStream = ToSolted(inputStream, solt);
        return ComputeHash(soltedStream);
    }
    protected virtual byte[] ToSolted(byte[] source, byte[] solt)
    {
        return source.Concat(solt).ToArray();
    }
    protected virtual Stream ToSolted(Stream source, byte[] solt)
    {
        throw new NotImplementedException();
    }
}
