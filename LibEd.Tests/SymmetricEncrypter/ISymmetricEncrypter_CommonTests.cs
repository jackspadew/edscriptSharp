namespace LibEd.Tests;

using Xunit;
using LibEd.SymmetricEncrypter;
using LibEd.SymmetricAlgorithmAdapter;

#pragma warning disable xUnit1026 // Unused arguments

public class ISymmetricEncrypter_CommonTests
{
    public class SymmetricEncrypter_ForTestSystemCryptographyAES : SymmetricEncrypterBase
    {
        protected override ISymmetricAlgorithmAdapter Algorithm => new SymmetricAlgorithmAdapter.SystemCryptography.AES();
    }
    public class SymmetricEncrypter_ForTestMultipleEncryptionBySystemCryptographyAES : MultipleSymmetricEncrypterBase
    {
        protected override int MultipleCryptionCount => 10;
        protected override List<ISymmetricAlgorithmAdapter> AlgorithmCandidateList => new(){
            new SymmetricAlgorithmAdapter.SystemCryptography.AES(),
            new SymmetricAlgorithmAdapter.SystemCryptography.AES(),
        };
    }
    public class SymmetricEncrypter_ForTestBouncyCastleAES : SymmetricEncrypterBase
    {
        protected override ISymmetricAlgorithmAdapter Algorithm => new SymmetricAlgorithmAdapter.BouncyCastle.AES();
    }
    public class SymmetricEncrypter_ForTestMultipleEncryptionByBouncyCastleAES : MultipleSymmetricEncrypterBase
    {
        protected override int MultipleCryptionCount => 10;
        protected override List<ISymmetricAlgorithmAdapter> AlgorithmCandidateList => new(){
            new SymmetricAlgorithmAdapter.BouncyCastle.AES(),
            new SymmetricAlgorithmAdapter.BouncyCastle.AES(),
        };
    }
    public static IEnumerable<object[]> ISymmetricEncrypterObjects()
    {
        yield return new object[] { new SymmetricEncrypter_ForTestSystemCryptographyAES(), "SymmetricEncrypter_ForTestSystemCryptographyAES" };
        yield return new object[] { new SymmetricEncrypter_ForTestMultipleEncryptionBySystemCryptographyAES(), "SymmetricEncrypter_ForTestMultipleEncryptionBySystemCryptographyAES" };
        yield return new object[] { new SymmetricEncrypter_ForTestBouncyCastleAES(), "SymmetricEncrypter_ForTestBouncyCastleAES" };
        yield return new object[] { new SymmetricEncrypter_ForTestMultipleEncryptionByBouncyCastleAES(), "SymmetricEncrypter_ForTestMultipleEncryptionByBouncyCastleAES" };
        yield return new object[] { new RandomizedMultipleEncrypter(1,1,1,1), "RandomizedMultipleEncrypter_OneAlgo" };
        yield return new object[] { new RandomizedMultipleEncrypter(1,1,1,2), "RandomizedMultipleEncrypter_Two" };
        yield return new object[] { new RandomizedMultipleEncrypter(1,1,1,3), "RandomizedMultipleEncrypter_Three" };
        yield return new object[] { new RandomizedMultipleEncrypter(1,1,1,100), "RandomizedMultipleEncrypter_HighMultiple" };
    }
    public static byte[] exampleBytes = {0,1,2,3};
    public static byte[] exampleKey = new byte[32];
    public static byte[] exampleIV = new byte[16];

    [Theory]
    [MemberData(nameof(ISymmetricEncrypterObjects))]
    public void EncryptBytes_ReturnNotEmpty(ISymmetricEncrypter encrypter, string className)
    {
        byte[] value = encrypter.Encrypt(exampleBytes, exampleKey, exampleIV);
        Assert.NotEmpty(value);
    }
    [Theory]
    [MemberData(nameof(ISymmetricEncrypterObjects))]
    public void DecryptBytes_ReturnSourceBytes(ISymmetricEncrypter encrypter, string className)
    {
        byte[] encrypted = encrypter.Encrypt(exampleBytes, exampleKey, exampleIV);
        byte[] value = encrypter.Decrypt(encrypted, exampleKey, exampleIV);
        Assert.Equal(exampleBytes, value);
    }
    [Theory]
    [MemberData(nameof(ISymmetricEncrypterObjects))]
    public void EncryptStream_ReturnNotEmpty(ISymmetricEncrypter encrypter, string className)
    {
        MemoryStream inputStream = new();
        MemoryStream outputStream = new();
        inputStream.Write(exampleBytes, 0, exampleBytes.Length);
        inputStream.Position = 0;
        encrypter.Encrypt(inputStream, outputStream, exampleKey, exampleIV);
        byte[] value = outputStream.ToArray();
        Assert.NotEmpty(value);
    }
    [Theory]
    [MemberData(nameof(ISymmetricEncrypterObjects))]
    public void DecryptStream_ReturnSourceBytes(ISymmetricEncrypter encrypter, string className)
    {
        MemoryStream encInputStream = new();
        MemoryStream encOutputStream = new();
        encInputStream.Write(exampleBytes, 0, exampleBytes.Length);
        encInputStream.Position = 0;
        encrypter.Encrypt(encInputStream, encOutputStream, exampleKey, exampleIV);
        byte[] encryptedBytes = encOutputStream.ToArray();
        MemoryStream decInputStream = new();
        MemoryStream decOutputStream = new();
        decInputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
        decInputStream.Position = 0;
        encrypter.Decrypt(decInputStream, decOutputStream, exampleKey, exampleIV);
        byte[] value = decOutputStream.ToArray();
        Assert.Equal(exampleBytes, value);
    }
    [Theory]
    [MemberData(nameof(ISymmetricEncrypterObjects))]
    public void EncryptStreamAndEncryptBytes_ReturnBytesIsEqual(ISymmetricEncrypter encrypter, string className)
    {
        byte[] valueByBytesEncryption = encrypter.Encrypt(exampleBytes, exampleKey, exampleIV);
        MemoryStream inputStream = new();
        MemoryStream outputStream = new();
        inputStream.Write(exampleBytes, 0, exampleBytes.Length);
        inputStream.Position = 0;
        encrypter.Encrypt(inputStream, outputStream, exampleKey, exampleIV);
        byte[] valueByStreamEncryption = outputStream.ToArray();
        Assert.Equal(valueByBytesEncryption, valueByStreamEncryption);
    }
}
