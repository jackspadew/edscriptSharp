namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;

public class Camellia : BouncyCastleEngineAdapterBase, ISymmetricAlgorithmAdapter
{
    protected override IBlockCipher BCCryptoEngine => new CamelliaEngine();
}
