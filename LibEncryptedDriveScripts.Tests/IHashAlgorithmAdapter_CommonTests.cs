namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.HashAlgorithmAdapter;
using System.Collections;

#pragma warning disable xUnit1026 // Unused arguments

public class IHashAlgorithmAdapter_CommonTests
{
    public static IEnumerable<object[]> HashAlgorithObjects()
    {
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.SHA3(), "BouncyCastle.SHA3" };
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHash_WillReturnThatHaveEnoughLength(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] hash = hashAlgo.ComputeHash(new byte[]{0},new byte[]{0});
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashBySomeValues_WillReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] dataOne = {0,1};
        byte[] dataTwo = {1,1};
        byte[] hashOne = hashAlgo.ComputeHash(dataOne);
        byte[] hashTwo = hashAlgo.ComputeHash(dataTwo);
        Assert.NotEqual(hashOne,hashTwo);
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashAnotherSolt_WillReturnAnotherHash(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] data = {0,1,2,3,4,5,6,7};
        byte[] soltOne = {0};
        byte[] soltTwo = {1};
        byte[] hashSoltOne = hashAlgo.ComputeHash(data, soltOne);
        byte[] hashSoltTwo = hashAlgo.ComputeHash(data, soltTwo);
        Assert.NotEqual(hashSoltOne, hashSoltTwo);
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
