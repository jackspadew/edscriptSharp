namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.BouncyCastle;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter.SystemCryptography;

#pragma warning disable xUnit1026 // Unused arguments

public class ISymmetricAlgorithmAdapterCommonTests
{
    public static byte[] exampleBytes = new byte[]{0,1,2,3};
    public static byte[] exampleKey = new byte[32];
    public static byte[] exampleIV = new byte[16];
    public static IEnumerable<object[]> EncryptAlgorithObjects()
    {
        yield return new object[] { new SymmetricAlgorithmAdapter.BouncyCastle.AES(), "BouncyCastleAdapter.AES" };
        yield return new object[] { new SymmetricAlgorithmAdapter.BouncyCastle.Camellia(), "BouncyCastleAdapter.Camellia" };
        yield return new object[] { new SymmetricAlgorithmAdapter.SystemCryptography.AES(), "CryptographyAdapter.AES" };
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void Encrypt_ThenOutputSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] outputBytes = encryptAlgo.EncryptBytes(exampleBytes,exampleKey,exampleIV);
        Assert.NotEmpty(outputBytes);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptThenDecrypt_BeforeAndAfterIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] encryptedBytes = encryptAlgo.EncryptBytes(exampleBytes,exampleKey,exampleIV);
        byte[] decryptedBytes = encryptAlgo.DecryptBytes(encryptedBytes,exampleKey,exampleIV);
        Assert.Equal(exampleBytes, decryptedBytes);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptWithStream_BeforeAndAfterIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        MemoryStream inputEncryptStream = new MemoryStream(exampleBytes);
        MemoryStream encryptedStream = new MemoryStream();
        encryptAlgo.Encrypt(inputEncryptStream, encryptedStream, exampleKey, exampleIV);
        byte[] encryptedBytes = encryptedStream.ToArray();
        MemoryStream inputDecryptStream = new MemoryStream(encryptedBytes);
        MemoryStream outputStream = new MemoryStream();
        encryptAlgo.Decrypt(inputDecryptStream, outputStream, exampleKey, exampleIV);
        byte[] decryptedBytes = outputStream.ToArray();
        Assert.Equal(exampleBytes, decryptedBytes);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void WriteToCreatedWritableEncryptStream_NotThrowAndOutSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        MemoryStream outputStream = new MemoryStream();
        using(Stream writableStream = encryptAlgo.CreateWritableEncryptStream(outputStream, exampleKey, exampleIV))
        {
            writableStream.Write(exampleBytes, 0, exampleBytes.Length);
        }
        Assert.NotEmpty(outputStream.ToArray());
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void WriteToCreatedWritableDecryptStream_NotThrowAndOutSomething(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] encryptedBytes = encryptAlgo.EncryptBytes(exampleBytes,exampleKey,exampleIV);
        MemoryStream outputStream = new MemoryStream();
        using(Stream writableStream = encryptAlgo.CreateWritableDecryptStream(outputStream, exampleKey, exampleIV))
        {
            writableStream.Write(encryptedBytes, 0, encryptedBytes.Length);
        }
        Assert.NotEmpty(outputStream.ToArray());
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptThenDecryptByCreatedWritableEncryptStream_InputAndOutputIsEqual(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] exampleBytes = new byte[102400];
        byte[] encrypted;
        byte[] decrypted;
        MemoryStream encryptOutputStream = new MemoryStream();
        Stream writableEncryptStream = encryptAlgo.CreateWritableEncryptStream(encryptOutputStream, exampleKey, exampleIV);
        writableEncryptStream.Write(exampleBytes, 0, exampleBytes.Length);
        writableEncryptStream.Dispose();
        encrypted = encryptOutputStream.ToArray();
        encryptOutputStream.Dispose();
        MemoryStream decryptOutputStream = new MemoryStream();
        Stream writableDecryptStream = encryptAlgo.CreateWritableDecryptStream(decryptOutputStream, exampleKey, exampleIV);
        writableDecryptStream.Write(encrypted, 0, encrypted.Length);
        writableDecryptStream.Dispose();
        decrypted = decryptOutputStream.ToArray();
        decryptOutputStream.Dispose();
        Assert.Equal(exampleBytes, decrypted);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void EncryptWithAnotherArgumentType_ReturnSameValue(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] outputBytesByBytes = encryptAlgo.EncryptBytes(exampleBytes,exampleKey,exampleIV);
        MemoryStream encryptOutputStream = new MemoryStream();
        Stream writableEncryptStream = encryptAlgo.CreateWritableEncryptStream(encryptOutputStream, exampleKey, exampleIV);
        writableEncryptStream.Write(exampleBytes, 0, exampleBytes.Length);
        writableEncryptStream.Dispose();
        byte[] outputBytesByCreatedStream = encryptOutputStream.ToArray();
        MemoryStream inputEncryptStream = new MemoryStream(exampleBytes);
        MemoryStream encryptedStream = new MemoryStream();
        encryptAlgo.Encrypt(inputEncryptStream, encryptedStream, exampleKey, exampleIV);
        byte[] outputBytesByInOutStreams = encryptedStream.ToArray();
        Assert.Equal(outputBytesByBytes,outputBytesByCreatedStream);
        Assert.Equal(outputBytesByBytes,outputBytesByInOutStreams);
    }

    [Theory]
    [MemberData(nameof(EncryptAlgorithObjects))]
    public void MultiEncryptionStream_DecryptedIsInput(ISymmetricAlgorithmAdapter encryptAlgo, string className)
    {
        byte[] encryptedBytes = [];
        byte[] decryptedBytes = [];
        int multiple = 3;
        using(MemoryStream encryptedOutputStream = new())
        {
            Stream[] encryptStreamArray = new Stream[multiple];
            encryptStreamArray[multiple - 1] = encryptAlgo.CreateWritableEncryptStream(encryptedOutputStream, exampleKey, exampleIV);
            for(int i=multiple-2; i>=0; i--)
            {
                encryptStreamArray[i] = encryptAlgo.CreateWritableEncryptStream(encryptStreamArray[i+1], exampleKey, exampleIV);
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
            decryptStreamArray[multiple - 1] = encryptAlgo.CreateWritableDecryptStream(decryptedOutputStream, exampleKey, exampleIV);
            for(int i=multiple-2; i>=0; i--)
            {
                decryptStreamArray[i] = encryptAlgo.CreateWritableDecryptStream(decryptStreamArray[i+1], exampleKey, exampleIV);
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
