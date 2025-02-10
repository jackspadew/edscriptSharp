namespace LibEd.HashAlgorithmAdapter.BouncyCastle;

using LibEd.HashAlgorithmAdapter;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

public class SHA3 : BouncyCastleDigestAdaptorBase, IHashAlgorithmAdapter
{
    private int[] _legalBitLengthList = new int[]{ 224, 256, 384, 512 };
    public override int[] LegalBitLengthList => _legalBitLengthList;

    public SHA3()
    {
        this.BitLength = 512;
    }
    public SHA3(int bitLength)
    {
        this.BitLength = bitLength;
    }

    protected override IDigest CreateIDigestObj(int bitLength)
    {
        return new Sha3Digest(bitLength);
    }
}
