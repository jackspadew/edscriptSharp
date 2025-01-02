namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.SystemCryptography;

#pragma warning disable xUnit1026 // Unused arguments

public class ISymmetricAlgorithmAdapterCommonTests
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

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptWithStream_BeforeAndAfterIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        MemoryStream inputEncryptStream = new MemoryStream(inputBytes);
        MemoryStream encryptedStream = new MemoryStream();
        encryptAlgo.Encrypt(inputEncryptStream, encryptedStream, key, iv);
        byte[] encryptedBytes = encryptedStream.ToArray();
        MemoryStream inputDecryptStream = new MemoryStream(encryptedBytes);
        MemoryStream outputStream = new MemoryStream();
        encryptAlgo.Decrypt(inputDecryptStream, outputStream, key, iv);
        byte[] decryptedBytes = outputStream.ToArray();
        Assert.Equal(inputBytes, decryptedBytes);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void WriteToCreatedWritableEncryptStream_NotThrowAndOutSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        MemoryStream outputStream = new MemoryStream();
        using(Stream writableStream = encryptAlgo.CreateWritableEncryptStream(outputStream, key, iv))
        {
            writableStream.Write(inputBytes, 0, inputBytes.Length);
        }
        Assert.NotEmpty(outputStream.ToArray());
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void WriteToCreatedWritableDecryptStream_NotThrowAndOutSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[] {0,1,2,3};
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        byte[] encryptedBytes = encryptAlgo.EncryptBytes(inputBytes,key,iv);
        MemoryStream outputStream = new MemoryStream();
        using(Stream writableStream = encryptAlgo.CreateWritableDecryptStream(outputStream, key, iv))
        {
            writableStream.Write(encryptedBytes, 0, encryptedBytes.Length);
        }
        Assert.NotEmpty(outputStream.ToArray());
    }
}
