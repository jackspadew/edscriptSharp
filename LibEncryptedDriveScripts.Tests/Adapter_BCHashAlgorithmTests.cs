namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.HashAlgorithmAdapter;

public class Adapter_BCHashAlgorithmTests
{
    public static IEnumerable<object[]> HashAlgorithObjects()
    {
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.SHA3() };
    }

    [Fact]
    public void ComputeHashBySHA3_HashLengthIs512bits()
    {
        IHashAlgorithmAdapter hashAlgo = new HashAlgorithmAdapter.BouncyCastle.SHA3();
        byte[] hash = hashAlgo.ComputeHash(new byte[0], new byte[0]);
        Assert.Equal((512/8), hash.Length);
    }

    [Theory]
    [InlineData(512)]
    [InlineData(224)]
    [InlineData(256)]
    [InlineData(384)]
    public void ComputeHashBySHA3_HashLengthIsGivenValue(int value)
    {
        IHashAlgorithmAdapter hashAlgo = new HashAlgorithmAdapter.BouncyCastle.SHA3(value);
        byte[] hash = hashAlgo.ComputeHash(new byte[0], new byte[0]);
        Assert.Equal((value/8), hash.Length);
    }

    [Theory]
    [InlineData(0)][InlineData(123)][InlineData(1024)]
    public void InitSHA3WithIllegalValue_WillThrow(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new HashAlgorithmAdapter.BouncyCastle.SHA3(value));
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHashAnotherSolt_WillOutAnotherHash(IHashAlgorithmAdapter hashAlgo)
    {
        byte[] data = {0,1,2,3,4,5,6,7};
        byte[] soltOne = {0};
        byte[] soltTwo = {1};
        byte[] hashSoltOne = hashAlgo.ComputeHash(data, soltOne);
        byte[] hashSoltTwo = hashAlgo.ComputeHash(data, soltTwo);
        Assert.NotEqual(hashSoltOne, hashSoltTwo);
    }
}
