namespace LibEncryptedDriveScripts.Tests;

using Moq;
using Xunit;
using LibEncryptedDriveScripts.EdData;

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
}
