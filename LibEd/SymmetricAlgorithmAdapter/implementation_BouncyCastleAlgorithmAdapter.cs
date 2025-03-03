namespace LibEd.SymmetricAlgorithmAdapter.BouncyCastle;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;

public class AES : BouncyCastleEngineAdapterBase, ISymmetricAlgorithmAdapter
{
    protected override IBlockCipher BCCryptoEngine => new AesEngine();
}

public class Camellia : BouncyCastleEngineAdapterBase, ISymmetricAlgorithmAdapter
{
    protected override IBlockCipher BCCryptoEngine => new CamelliaEngine();
}

public class Twofish : BouncyCastleEngineAdapterBase, ISymmetricAlgorithmAdapter
{
    protected override IBlockCipher BCCryptoEngine => new TwofishEngine();
}

public class Serpent : BouncyCastleEngineAdapterBase, ISymmetricAlgorithmAdapter
{
    protected override IBlockCipher BCCryptoEngine => new SerpentEngine();
}

public class SM4 : BouncyCastleEngineAdapterBase, ISymmetricAlgorithmAdapter
{
    public override int LegalKeySize => 16;
    protected override IBlockCipher BCCryptoEngine => new SM4Engine();
}
