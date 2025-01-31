namespace LibEd.Tests;

using Moq;
using Xunit;
using LibEd.EdData;

#pragma warning disable xUnit1026 // Unused arguments

public class IEdDataCryptor_CommonTests
{
    private byte[] exampleBytes = new byte[]{0,1,2,3};
    private Mock<IMultipleKeyExchanger> GetMockedMultipleKeyExchanger()
    {
        var mock = new Mock<IMultipleKeyExchanger>();
        mock.SetupProperty(x => x.KeySeed, 0);
        mock.SetupProperty(x => x.IVSeed, 0);
        mock.SetupProperty(x => x.AlgorithmSeed, 0);
        mock.SetupProperty(x => x.Key, new byte[32]);
        mock.SetupProperty(x => x.IV, new byte[16]);
        return mock;
    }

    public static IEnumerable<object[]> IEdDataCryptorObjects()
    {
        yield return new object[] { new EdDataCryptor(1000), "EdDataCryptor" };
    }

    [Theory]
    [MemberData(nameof(IEdDataCryptorObjects))]
    public void EncryptAndDecrypt_ReturnSourceBytes(IEdDataCryptor cryptor, string className)
    {
        var mockMultiKey = GetMockedMultipleKeyExchanger();
        var encryptedBytes = cryptor.EncryptBytes(exampleBytes, mockMultiKey.Object);
        var resultBytes = cryptor.DecryptBytes(encryptedBytes, mockMultiKey.Object);
        Assert.NotEqual(exampleBytes, encryptedBytes);
        Assert.Equal(exampleBytes, resultBytes);
    }

    [Theory]
    [MemberData(nameof(IEdDataCryptorObjects))]
    public void EncryptAndDecryptByStream_ReturnSourceBytes(IEdDataCryptor cryptor, string className)
    {
        var mockMultiKey = GetMockedMultipleKeyExchanger();
        Stream sourceStream = new MemoryStream(exampleBytes);
        MemoryStream encryptedStream = new MemoryStream();
        cryptor.EncryptStream(sourceStream, encryptedStream, mockMultiKey.Object);
        byte[] encryptedBytes = encryptedStream.ToArray();
        encryptedStream = new MemoryStream(encryptedBytes);
        MemoryStream decryptedStream = new MemoryStream();
        cryptor.DecryptStream(encryptedStream, decryptedStream, mockMultiKey.Object);
        byte[] resultBytes = decryptedStream.ToArray();
        Assert.NotEqual(exampleBytes, encryptedBytes);
        Assert.Equal(exampleBytes, resultBytes);
    }
}
