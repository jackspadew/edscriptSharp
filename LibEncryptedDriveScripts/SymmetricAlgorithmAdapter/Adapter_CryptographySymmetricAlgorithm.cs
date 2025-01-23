namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.SystemCryptography;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

using System.IO;
using System.Security.Cryptography;

public class AES : SymmetricAlgorithmAdapterBase,ISymmetricAlgorithmAdapter
{
    public override int LegalIVSize {
        get { return 16; }
    }
    public override int LegalKeySize { get {return 32;}}

    public override void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        using (var encryptStream = CreateWritableEncryptStream(outputStream, key, iv))
        {
            inputStream.CopyTo(encryptStream);
        }
    }
    public override void Decrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        using (var decryptStream = CreateWritableDecryptStream(outputStream, key, iv))
        {
            inputStream.CopyTo(decryptStream);
        }
    }
    public override Stream CreateWritableEncryptStream(Stream outputStream, byte[] key, byte[] iv)
    {
        System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        return new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write, false);
    }
    public override Stream CreateWritableDecryptStream(Stream outputStream, byte[] key, byte[] iv)
    {
        System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        return new CryptoStream(outputStream, aes.CreateDecryptor(), CryptoStreamMode.Write, false);
    }
}
