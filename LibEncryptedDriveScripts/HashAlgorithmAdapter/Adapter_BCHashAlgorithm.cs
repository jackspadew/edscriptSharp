namespace LibEncryptedDriveScripts.HashAlgorithmAdapter.BouncyCastle;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;

using System.IO;
using Org.BouncyCastle.Crypto.Digests;

public class SHA3 : IHashAlgorithmAdapter
{
    public static int[] LegalBitLengthList = { 224, 256, 384, 512 };
    private int _bitLength = 512;
    public int BitLength {
        get { return _bitLength; }
        set {
            if(!isLegalBitLength(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "");
            }
            _bitLength = value;
        }
    }
    public SHA3() {}
    public SHA3(int bitLength)
    {
        this.BitLength = bitLength;
    }
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
        var sha3 = new Sha3Digest(BitLength);
        
        sha3.BlockUpdate(inputBytes, 0, inputBytes.Length);
        
        byte[] result = new byte[sha3.GetDigestSize()];
        sha3.DoFinal(result, 0);
        
        return result;
    }
    private byte[] ComputeBCSHA3Hash(Stream inputStream)
    {
        var sha3 = new Sha3Digest(BitLength);
        int bytesRead;
        byte[] buffer = new byte[1024];
        while((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            sha3.BlockUpdate(buffer, 0, bytesRead);
        }
        byte[] result = new byte[sha3.GetDigestSize()];
        sha3.DoFinal(result, 0);
        return result;
    }
    private bool isLegalBitLength(int bitLength)
    {
        foreach(int LegalBitLength in SHA3.LegalBitLengthList)
        {
            if(bitLength == LegalBitLength)
            {
                return true;
            }
        }
        return false;
    }
}
