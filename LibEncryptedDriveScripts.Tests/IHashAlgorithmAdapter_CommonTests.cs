namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.HashAlgorithmAdapter;

#pragma warning disable xUnit1026 // Unused arguments

public class IHashAlgorithmAdapter_CommonTests
{
    public static byte[] exampleBytes = {0,1,0,0};
    public static byte[] anotherBytes = {0,0,0,0};
    public static byte[] exampleSalt = {1,1};
    public static byte[] anotherSalt = {2,2};
    public static IEnumerable<object[]> HashAlgorithObjects()
    {
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.SHA3(), "BouncyCastle.SHA3" };
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHash_WillReturnThatHaveEnoughLength(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] hash = hashAlgo.ComputeHash(exampleBytes);
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashBySomeValues_WillReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] hashOne = hashAlgo.ComputeHash(exampleBytes);
        byte[] hashTwo = hashAlgo.ComputeHash(anotherBytes);
        Assert.NotEqual(hashOne,hashTwo);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashAnotherSalt_WillReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] hashSaltOne = hashAlgo.ComputeHash(exampleBytes, exampleSalt);
        byte[] hashSaltTwo = hashAlgo.ComputeHash(exampleBytes, anotherSalt);
        Assert.NotEqual(hashSaltOne, hashSaltTwo);
    }

    private Stream CreateFilledLargeDataStream(byte oneByte, int lengthMB)
    {
        MemoryStream mStream = new MemoryStream();
        byte[] bytes = Enumerable.Repeat(oneByte, 1024).ToArray();
        for(int i=0; i<1024*lengthMB; i++)
        {
            mStream.Write(bytes, 0, bytes.Length);
        }
        mStream.Position = 0;
        return mStream;
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashByStream_WillReturnHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        Stream stream = CreateFilledLargeDataStream(0,10);
        byte[] hash = hashAlgo.ComputeHash(stream);
        stream.Dispose();
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashByStreamWithSalt_ReturnHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        Stream stream = CreateFilledLargeDataStream(0,10);
        byte[] hash = hashAlgo.ComputeHash(stream, exampleSalt);
        stream.Dispose();
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashByStreamWithSaltAndWithoutSalt_ReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        Stream streamOne = CreateFilledLargeDataStream(0,10);
        byte[] hashSalted = hashAlgo.ComputeHash(streamOne, exampleSalt);
        Stream streamTwo = CreateFilledLargeDataStream(0,10);
        byte[] hash = hashAlgo.ComputeHash(streamTwo);
        Assert.NotEqual(hash, hashSalted);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashByStreamWithDifferentSalt_ReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        Stream streamOne = CreateFilledLargeDataStream(0,10);
        byte[] hashSalted = hashAlgo.ComputeHash(streamOne, exampleSalt);
        Stream streamTwo = CreateFilledLargeDataStream(0,10);
        byte[] hashAnother = hashAlgo.ComputeHash(streamTwo, anotherSalt);
        Assert.NotEqual(hashAnother, hashSalted);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashByTwoStream_WillReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        Stream streamOne = CreateFilledLargeDataStream(0,1);
        Stream streamTwo = CreateFilledLargeDataStream(1,1);
        byte[] hashOne = hashAlgo.ComputeHash(streamOne);
        byte[] hashTwo = hashAlgo.ComputeHash(streamTwo);
        streamOne.Dispose();
        streamTwo.Dispose();
        Assert.NotEqual(hashOne,hashTwo);
    }
}
