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

public class BLAKE3 : BouncyCastleDigestAdaptorBase, IHashAlgorithmAdapter
{
    private int[] _legalBitLengthList = new int[]{ 224, 256, 384, 512 };
    public override int[] LegalBitLengthList => _legalBitLengthList;

    public BLAKE3()
    {
        this.BitLength = 512;
    }
    public BLAKE3(int bitLength)
    {
        this.BitLength = bitLength;
    }

    protected override IDigest CreateIDigestObj(int bitLength)
    {
        return new Blake3Digest(bitLength);
    }
}

public class BLAKE2b : BouncyCastleDigestAdaptorBase, IHashAlgorithmAdapter
{
    private int[] _legalBitLengthList = new int[]{ 224, 256, 384, 512 };
    public override int[] LegalBitLengthList => _legalBitLengthList;

    public BLAKE2b()
    {
        this.BitLength = 512;
    }
    public BLAKE2b(int bitLength)
    {
        this.BitLength = bitLength;
    }

    protected override IDigest CreateIDigestObj(int bitLength)
    {
        return new Blake2bDigest(bitLength);
    }
}

public class Skein : BouncyCastleDigestAdaptorBase, IHashAlgorithmAdapter
{
    private int[] _legalBitLengthList = new int[]{ 224, 256, 384, 512 };
    public override int[] LegalBitLengthList => _legalBitLengthList;

    public Skein()
    {
        this.BitLength = 512;
    }
    public Skein(int bitLength)
    {
        this.BitLength = bitLength;
    }

    protected override IDigest CreateIDigestObj(int bitLength)
    {
        return new SkeinDigest(1024, bitLength);
    }
}

public class Whirlpool : BouncyCastleDigestAdaptorBase, IHashAlgorithmAdapter
{
    private int[] _legalBitLengthList = new int[]{ 512 };
    public override int[] LegalBitLengthList => _legalBitLengthList;

    public Whirlpool()
    {
        this.BitLength = 512;
    }

    protected override IDigest CreateIDigestObj(int bitLength)
    {
        return new WhirlpoolDigest();
    }
}
