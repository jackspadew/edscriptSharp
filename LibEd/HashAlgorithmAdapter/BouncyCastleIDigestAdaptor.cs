namespace LibEd.HashAlgorithmAdapter.BouncyCastle;

using LibEd.HashAlgorithmAdapter;

using System.IO;
using Org.BouncyCastle.Crypto;

public abstract class BouncyCastleDigestAdaptorBase : IHashAlgorithmAdapter
{
    public abstract int[] LegalBitLengthList { get; }
    protected int _bitLength;
    public virtual int BitLength {
        get { return _bitLength; }
        set {
            if(!isLegalBitLength(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "");
            }
            _bitLength = value;
        }
    }

    protected abstract IDigest CreateIDigestObj(int bitLength);
    public byte[] ComputeHash(byte[] inputBytes)
    {
        return ComputeBCSHA3Hash(inputBytes);
    }
    public byte[] ComputeHash(Stream inputStream)
    {
        return ComputeBCSHA3Hash(inputStream);
    }
    private byte[] ComputeBCSHA3Hash(byte[] inputBytes)
    {
        IDigest digest = CreateIDigestObj(BitLength);
        
        digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
        
        byte[] result = new byte[digest.GetDigestSize()];
        digest.DoFinal(result, 0);
        
        return result;
    }
    private byte[] ComputeBCSHA3Hash(Stream inputStream)
    {
        IDigest digest = CreateIDigestObj(BitLength);
        int bytesRead;
        byte[] buffer = new byte[1024];
        while((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            digest.BlockUpdate(buffer, 0, bytesRead);
        }
        byte[] result = new byte[digest.GetDigestSize()];
        digest.DoFinal(result, 0);
        return result;
    }
    protected bool isLegalBitLength(int bitLength)
    {
        foreach(int LegalBitLength in this.LegalBitLengthList)
        {
            if(bitLength == LegalBitLength)
            {
                return true;
            }
        }
        return false;
    }
}
