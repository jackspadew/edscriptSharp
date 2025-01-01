namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Paddings;

public class AES : ISymmetricAlgorithmAdapter
{
    public int LegalIVSize {
        get { return (new AesEngine()).GetBlockSize(); }
    }
    public int LegalKeySize { get {return 16;}}

    public Stream CreateDecryptStream(Stream inputStream, byte[] key, byte[] iv)
    {
        return CreateStream(false, inputStream, key, iv);
    }

    public Stream CreateEncryptStream(Stream inputStream, byte[] key, byte[] iv)
    {
        return CreateStream(true, inputStream, key, iv);
    }
    private Stream CreateStream(bool forEncryption, Stream inputStream, byte[] key, byte[] iv)
    {
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(key), iv));

        return new CipherStream(inputStream, cipher, cipher);
    }
}
