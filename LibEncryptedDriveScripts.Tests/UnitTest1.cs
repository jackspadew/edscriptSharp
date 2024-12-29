namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts;
using System.Security.Cryptography;

public class DressableCryptographicConverterTests
{
    private DressableCryptographicConverter converter;
    private HashAlgorithm hashAlgo_SHA2_256 = SHA256.Create();
    private SymmetricAlgorithm encryptAlgo_DES = DES.Create();
    public DressableCryptographicConverterTests()
    {
        converter = new DressableCryptographicConverter();
    }

    [Theory]
    [InlineData(new byte[] {0,0,0,0})]
    [InlineData(new byte[] {0,1,2,3})]
    [InlineData(new byte[] {0,1,2,3,4,5})]
    public void EncryptThenDecrypt_InputAndDecryptedBytesIsEqual(byte[] input)
    {
        byte[] encrypted = converter.Encrypt(input);
        byte[] decrypted = converter.Decrypt(encrypted);
        Assert.Equal(input.Length, decrypted.Length);
        Assert.Equal(input, decrypted);
    }

    [Fact]
    public void UsingSHA256HashAlgorithm_InputAndDecryptedIsEqual()
    {
        DressableCryptographicConverter converter = new DressableCryptographicConverter(hashAlgo_SHA2_256);
        byte[] input = new byte[] {0,1,2,3};
        byte[] encrypted = converter.Encrypt(input);
        byte[] decrypted = converter.Decrypt(encrypted);
        Assert.Equal(input.Length, decrypted.Length);
        Assert.Equal(input, decrypted);
    }
    [Fact]
    public void UsingSHA384HashAlgorithm_InputAndDecryptedIsEqual()
    {
        DressableCryptographicConverter converter = new DressableCryptographicConverter(encryptAlgo_DES);
        byte[] input = new byte[] {0,1,2,3};
        byte[] encrypted = converter.Encrypt(input);
        byte[] decrypted = converter.Decrypt(encrypted);
        Assert.Equal(input.Length, decrypted.Length);
        Assert.Equal(input, decrypted);
    }
}
