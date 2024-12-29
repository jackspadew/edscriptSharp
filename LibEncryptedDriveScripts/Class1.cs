namespace LibEncryptedDriveScripts;

using System.Security.Cryptography;
using System.Text;

public class DressableCryptographicConverter
{
    private byte[] _secretKey = new byte[32];
    private byte[] _iv = new byte[16];

    private HashAlgorithm hashAlgorithm = SHA512.Create();
    private SymmetricAlgorithm encryptAlgorithm = Aes.Create();

    public int SizeOfSecretKey {
        get {
            int maxKeySizeInBits = this.encryptAlgorithm.LegalKeySizes.Max(keySize => keySize.MaxSize);
            return maxKeySizeInBits / 8;
        }
    }
    public int SizeOfIV {
        get {
            return this.encryptAlgorithm.IV.Length;
        }
    }
    public DressableCryptographicConverter()
    {
        SetLegalSizeKeys();
    }
    public DressableCryptographicConverter(HashAlgorithm inputHashAlgorithm)
    {
        hashAlgorithm = inputHashAlgorithm;
        SetLegalSizeKeys();
    }
    public DressableCryptographicConverter(SymmetricAlgorithm inputEncrptAlgorithm)
    {
        encryptAlgorithm = inputEncrptAlgorithm;
        SetLegalSizeKeys();
    }
    public DressableCryptographicConverter(HashAlgorithm inputHashAlgorithm, SymmetricAlgorithm inputEncrptAlgorithm) : this()
    {
        hashAlgorithm = inputHashAlgorithm;
        encryptAlgorithm = inputEncrptAlgorithm;
        SetLegalSizeKeys();
    }
    private void SetLegalSizeKeys()
    {
        SecretKey = new byte[SizeOfSecretKey];
        IV = new byte[SizeOfIV];
    }
    public byte[] Encrypt(byte[] inputBytes)
    {
        byte[] encrypted;
        this.encryptAlgorithm.BlockSize = this.encryptAlgorithm.LegalBlockSizes.Max(keySize => keySize.MaxSize);
        ICryptoTransform encryptor = this.encryptAlgorithm.CreateEncryptor(this.SecretKey, this.IV);
        using (MemoryStream msEncrypt = new MemoryStream())
        {
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(inputBytes);
                csEncrypt.FlushFinalBlock();
            }
            encrypted = msEncrypt.ToArray();
        }
        return encrypted;
    }
    public byte[] Encrypt(string inputString)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        return this.Encrypt(inputBytes);
    }
    public byte[] Decrypt(byte[] encryptedBytes)
    {
        byte[] decrypted;
        ICryptoTransform decryptor = this.encryptAlgorithm.CreateDecryptor(this.SecretKey, this.IV);
        using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
        {
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (MemoryStream decryptedStream = new MemoryStream())
                {
                    csDecrypt.CopyTo(decryptedStream);
                    decrypted = decryptedStream.ToArray();
                }
            }
        }
        return decrypted;
    }
    private byte[] SecretKey {
        set {
            byte[] tmpBytes = hashAlgorithm.ComputeHash(value);
            Array.Resize( ref tmpBytes, SizeOfSecretKey);
            this._secretKey = tmpBytes;
        }
        get {
            return this._secretKey;
        }
    }
    private byte[] IV {
        set {
            byte[] tmpBytes = hashAlgorithm.ComputeHash(value);
            Array.Resize( ref tmpBytes, SizeOfIV);
            this._iv = tmpBytes;
        }
        get {
            return this._iv;
        }
    }
}
