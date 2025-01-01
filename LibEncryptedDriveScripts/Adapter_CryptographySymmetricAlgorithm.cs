namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.SystemCryptography;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

using System.IO;
using System.Security.Cryptography;

public class AES : ISymmetricAlgorithmAdapter
{
    public int LegalIVSize {
        get { return 16; }
    }
    public int LegalKeySize { get {return 32;}}

    public Stream CreateDecryptStream(Stream inputStream, byte[] key, byte[] iv)
    {
        return CreateAesDecryptedStream(inputStream, key, iv);
    }

    public Stream CreateEncryptStream(Stream inputStream, byte[] key, byte[] iv)
    {
        return CreateAesEncryptedStream(inputStream, key, iv);
    }
    private Stream CreateAesEncryptedStream(Stream inputStream, byte[] key, byte[] iv)
    {
        System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        Stream outputStream = new MemoryStream();
        using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
        using (var cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write, true))
        {
            inputStream.CopyTo(cryptoStream);
        }
        outputStream.Position = 0;
        return outputStream;
    }
    private Stream CreateAesDecryptedStream(Stream inputStream, byte[] key, byte[] iv)
    {
        System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        Stream decryptStream = new CryptoStream(inputStream, aes.CreateDecryptor(aes.Key,aes.IV), CryptoStreamMode.Read, true);
        return decryptStream;
    }
}
