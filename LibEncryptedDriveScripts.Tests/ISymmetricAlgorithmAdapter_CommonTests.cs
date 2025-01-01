namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.SystemCryptography;

#pragma warning disable xUnit1026 // Unused arguments

public class BCSymmetricAlgorithmTests
{
    public static IEnumerable<object[]> EncryptAlgorithObjects()
    {
        yield return new object[] { new SymmetricAlgorithmAdapter.BouncyCastle.AES(), "BouncyCastleAdapter.AES" };
        yield return new object[] { new SymmetricAlgorithmAdapter.SystemCryptography.AES(), "CryptographyAdapter.AES" };
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void Encrypt_ThenOutputSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        byte[] outputBytes = encryptAlgo.EncryptBytes(inputBytes,key,iv);
        Assert.NotEmpty(outputBytes);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptThenDecrypt_BeforeAndAfterIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        byte[] encryptedBytes = encryptAlgo.EncryptBytes(inputBytes,key,iv);
        byte[] decryptedBytes = encryptAlgo.DecryptBytes(encryptedBytes,key,iv);
        Assert.Equal(inputBytes, decryptedBytes);
    }
}
