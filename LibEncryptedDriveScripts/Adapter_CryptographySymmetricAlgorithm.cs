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
        System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        using (var encryptStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            inputStream.CopyTo(encryptStream);
        }
    }
    public override void Decrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        using (var decryptStream = new CryptoStream(outputStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            inputStream.CopyTo(decryptStream);
        }
    }
}
