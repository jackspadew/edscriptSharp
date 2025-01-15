namespace LibEncryptedDriveScripts.HashCalculator;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;
using LibEncryptedDriveScripts.SaltStream;

public abstract class HashCalculatorBase : IHashCalculator
{
    protected virtual IHashAlgorithmAdapter Algorithm => new HashAlgorithmAdapter.BouncyCastle.SHA3();
    protected virtual byte[] ComputeHashByGivenAlgorithm(IHashAlgorithmAdapter hashAlgorithm, byte[] inputBytes)
    {
        return hashAlgorithm.ComputeHash(inputBytes);
    }
    protected virtual byte[] ComputeHashByGivenAlgorithm(IHashAlgorithmAdapter hashAlgorithm, Stream inputStream)
    {
        return hashAlgorithm.ComputeHash(inputStream);
    }
    public virtual byte[] ComputeHash(byte[] inputBytes, byte[] salt, int stretchCount)
    {
        byte[] saltedBytes = ToSaltedBytes(inputBytes, salt);
        return ComputeHash(saltedBytes, stretchCount);
    }

    public virtual byte[] ComputeHash(byte[] inputBytes, int stretchCount = 1)
    {
        CheckStretchingCount(stretchCount);
        byte[] tmpHash = inputBytes;
        for(int i=0; i < stretchCount; i++)
        {
            tmpHash = ComputeHashByGivenAlgorithm(Algorithm, tmpHash);
        }
        return tmpHash;
    }

    public virtual byte[] ComputeHash(Stream inputStream, byte[] salt, int stretchCount = 1)
    {
        Stream saltedStream = ToSaltedStream(inputStream, salt);
        return ComputeHash(saltedStream, stretchCount);
    }

    public virtual byte[] ComputeHash(Stream inputStream, int stretchCount = 1)
    {
        CheckStretchingCount(stretchCount);
        byte[] tmpHash = ComputeHashByGivenAlgorithm(Algorithm, inputStream);
        if(stretchCount == 1) return tmpHash;
        return ComputeHash(tmpHash, stretchCount-1);
    }

    protected virtual byte[] ToSaltedBytes(byte[] inputBytes, byte[] salt)
    {
        MemoryStream inputStream = new(inputBytes);
        SaltStream saltStream = new(inputStream, salt, true);
        MemoryStream outputStream = new();
        saltStream.CopyTo(outputStream);
        byte[] result = outputStream.ToArray();
        return result;
    }
    protected virtual Stream ToSaltedStream(Stream inputStream, byte[] salt)
    {
        SaltStream saltStream = new(inputStream, salt, true);
        return saltStream;
    }
    private void CheckStretchingCount(int stretchCount)
    {
        if(stretchCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stretchCount), "The number of stretches must be at least once.");
        }
    }
}
