namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.BouncyCastleAdapter;

#pragma warning disable xUnit1026 // Unused arguments

public class BCSymmetricAlgorithmTests
{
    public static IEnumerable<object[]> EncryptAlgorithObjects()
    {
        yield return new object[] { new BouncyCastleAdapter.AES(), "BouncyCastleAdapter.AES" };
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void Encrypt_ThenOutputSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        MemoryStream inputStream = new MemoryStream(inputBytes);
        MemoryStream outputStream = new MemoryStream();
        var EncryptedStream = encryptAlgo.CreateEncryptStream(inputStream,key,iv);
        EncryptedStream.CopyTo(outputStream);
        var encryptedData = outputStream.ToArray();
        Assert.NotEmpty(encryptedData);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptThenDecrypt_BeforeAndAfterIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        MemoryStream inputStream = new MemoryStream(inputBytes);
        MemoryStream outputStream = new MemoryStream();
        var EncryptedStream = encryptAlgo.CreateEncryptStream(inputStream,key,iv);
        var DecryptedStream = encryptAlgo.CreateDecryptStream(EncryptedStream,key,iv);
        DecryptedStream.CopyTo(outputStream);
        byte[] outputBytes = outputStream.ToArray();
        Assert.Equal(outputBytes, inputBytes);
    }
}
