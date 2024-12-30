namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.BouncyCastleAdapter;

public class Adapter_BCHashAlgorithmTests
{
    public static IEnumerable<object[]> HashAlgorithObjects()
    {
        yield return new object[] { new BouncyCastleAdapter.SHA3() };
    }

    [Fact]
    public void ComputeHashBySHA3_HashLengthIs512bits()
    {
        IHashAlgorithmAdapter hashAlgo = new BouncyCastleAdapter.SHA3();
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
        IHashAlgorithmAdapter hashAlgo = new SHA3(value);
        byte[] hash = hashAlgo.ComputeHash(new byte[0], new byte[0]);
        Assert.Equal((value/8), hash.Length);
    }

    [Theory]
    [InlineData(0)][InlineData(123)][InlineData(1024)]
    public void InitSHA3WithIllegalValue_WillThrow(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SHA3(value));
    }
}