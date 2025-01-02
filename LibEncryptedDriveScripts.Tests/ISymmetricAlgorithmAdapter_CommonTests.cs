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

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptThenDecryptByCreatedWritableEncryptStream_InputAndOutputIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] inputBytes = new byte[1024];
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        byte[] encrypted;
        byte[] decrypted;
        MemoryStream encryptOutputStream = new MemoryStream();
        Stream writableEncryptStream = encryptAlgo.CreateWritableEncryptStream(encryptOutputStream, key, iv);
        writableEncryptStream.Write(inputBytes, 0, inputBytes.Length);
        writableEncryptStream.Dispose();
        encrypted = encryptOutputStream.ToArray();
        encryptOutputStream.Dispose();
        MemoryStream decryptOutputStream = new MemoryStream();
        Stream writableDecryptStream = encryptAlgo.CreateWritableDecryptStream(decryptOutputStream, key, iv);
        writableDecryptStream.Write(encrypted, 0, encrypted.Length);
        writableDecryptStream.Dispose();
        decrypted = decryptOutputStream.ToArray();
        decryptOutputStream.Dispose();
        Assert.Equal(inputBytes, decrypted);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void MultiEncryptionStream_DecryptedIsInput(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] iv = new byte[encryptAlgo.LegalIVSize];
        byte[] key = new byte[encryptAlgo.LegalKeySize];
        byte[] encryptedBytes = [];
        byte[] decryptedBytes = [];
        int multiple = 3;
        using(MemoryStream encryptedOutputStream = new())
        {
            Stream[] encryptStreamArray = new Stream[multiple];
            encryptStreamArray[multiple - 1] = encryptAlgo.CreateWritableEncryptStream(encryptedOutputStream, key, iv);
            for(int i=multiple-2; i>=0; i--)
            {
                encryptStreamArray[i] = encryptAlgo.CreateWritableEncryptStream(encryptStreamArray[i+1], key, iv);
            }
            Stream encryptFirstStream = encryptStreamArray[0];
            for(int i=0; i < 655360; i++) // 5MB
            {
                byte[] dataBlock = new byte[8];
                encryptFirstStream.Write(dataBlock, 0, dataBlock.Length);
            }
            encryptFirstStream.Dispose();
            encryptedBytes = encryptedOutputStream.ToArray();
        }
        Assert.InRange(encryptedBytes.Length, 5242880*0.9, 5242880*1.1);
        using(MemoryStream decryptedOutputStream = new())
        {
            Stream[] decryptStreamArray = new Stream[multiple];
            decryptStreamArray[multiple - 1] = encryptAlgo.CreateWritableDecryptStream(decryptedOutputStream, key, iv);
            for(int i=multiple-2; i>=0; i--)
            {
                decryptStreamArray[i] = encryptAlgo.CreateWritableDecryptStream(decryptStreamArray[i+1], key, iv);
            }
            Stream decryptFirstStream = decryptStreamArray[0];
            decryptFirstStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decryptFirstStream.Dispose();
            decryptedBytes = decryptedOutputStream.ToArray();
        }
        for(int i=0; i<decryptedBytes.Length; i++)
        {
            if(decryptedBytes[i] != 0)
            {
                Assert.Fail($"A byte at index {i} is not 0.");
            }
        }
    }
}
