namespace LibEncryptedDriveScripts.HashAlgorithmAdapter.BouncyCastle;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;

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
    public byte[] ComputeHash(byte[] inputBytes, byte[] solt)
    {
        return ComputeBCSHA3Hash(inputBytes.Concat(solt).ToArray());
    }
    private byte[] ComputeBCSHA3Hash(byte[] inputBytes)
    {
        var sha3 = new Sha3Digest(BitLength);
        
        sha3.BlockUpdate(inputBytes, 0, inputBytes.Length);
        
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
