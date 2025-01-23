namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto;

public abstract class BouncyCastleEngineAdapterBase : SymmetricAlgorithmAdapterBase,ISymmetricAlgorithmAdapter
{
    protected abstract IBlockCipher BCCryptoEngine {get;}
    public override int LegalIVSize => BCCryptoEngine.GetBlockSize();
    public override int LegalKeySize => 32;

    public override void Decrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        using (var decryptStream = CreateStream(false, outputStream, key, iv))
        {
            inputStream.CopyTo(decryptStream);
        }
    }
    public override void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        using (Stream encryptStream = CreateStream(true, outputStream, key, iv))
        {
            inputStream.CopyTo(encryptStream);
        }
    }
    private Stream CreateStream(bool forEncryption, Stream outputStream, byte[] key, byte[] iv)
    {
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(BCCryptoEngine));
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(key), iv));

        return new CipherStream(outputStream, cipher, cipher);
    }
    public override Stream CreateWritableDecryptStream(Stream outputStream, byte[] key, byte[] iv)
    {
        return CreateStream(false, outputStream, key, iv);
    }
    public override Stream CreateWritableEncryptStream(Stream outputStream, byte[] key, byte[] iv)
    {
        return CreateStream(true, outputStream, key, iv);
    }
}
