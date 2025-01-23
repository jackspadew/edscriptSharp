namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Paddings;

public abstract class BouncyCastleEngineAdapterBase : SymmetricAlgorithmAdapterBase,ISymmetricAlgorithmAdapter
{
    public override int LegalIVSize {
        get { return (new AesEngine()).GetBlockSize(); }
    }
    public override int LegalKeySize { get {return 32;}}

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
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
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
