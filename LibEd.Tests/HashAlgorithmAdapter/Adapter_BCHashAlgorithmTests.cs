namespace LibEd.Tests;

using Xunit;
using LibEd.HashAlgorithmAdapter;

public class Adapter_BCHashAlgorithmTests
{
    [Fact]
    public void ComputeHashBySHA3_HashLengthIs512bits()
    {
        IHashAlgorithmAdapter hashAlgo = new HashAlgorithmAdapter.BouncyCastle.SHA3();
        byte[] hash = hashAlgo.ComputeHash(new byte[1]);
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
        byte[] hash = hashAlgo.ComputeHash(new byte[1]);
        Assert.Equal((value/8), hash.Length);
    }

    [Theory]
    [InlineData(0)][InlineData(123)][InlineData(1024)]
    public void InitSHA3WithIllegalValue_WillThrow(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new HashAlgorithmAdapter.BouncyCastle.SHA3(value));
    }
}
