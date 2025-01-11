namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.HashCalculator;
using LibEncryptedDriveScripts.HashAlgorithmAdapter;

#pragma warning disable xUnit1026 // Unused arguments

public class IHashCalculator_Tests
{
    public class HashCalculator_ForTest : HashCalculatorBase, IHashCalculator
    {
        private IHashAlgorithmAdapter _algorithm;
        protected override IHashAlgorithmAdapter Algorithm => _algorithm;
        public HashCalculator_ForTest(IHashAlgorithmAdapter hashAlgorithm)
        {
            _algorithm = hashAlgorithm;
        }
    }
    public static byte[] exampleBytes = {0,1,0,0};
    public static MemoryStream exampleStream => new(exampleBytes);
    public static byte[] anotherBytes = {0,0,0,0};
    public static MemoryStream anotherStream => new(anotherBytes);
    public static byte[] exampleSalt = {0,0};
    public static IEnumerable<object[]> IHashCalculatorObjects()
    {
        yield return new object[] { new HashCalculator_ForTest(new HashAlgorithmAdapter.BouncyCastle.SHA3()), "ExampleHashCalculator_WithBcSha3" };
        yield return new object[] { new DynamicHashCalculator(new HashAlgorithmAdapter.BouncyCastle.SHA3()), "DynamicHashCalculator_WithBcSha3" };
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHash_WillReturnThatHaveEnoughLength(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleBytes);
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashByStream_WillReturnThatHaveEnoughLength(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleStream);
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashAnotherBytes_WillReturnAnotherHash(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleBytes);
        byte[] hashAnother = hashCalculator.ComputeHash(anotherBytes);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashAnotherStream_WillReturnAnotherHash(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleStream);
        byte[] hashAnother = hashCalculator.ComputeHash(anotherStream);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashWithStretching_WillReturnAnotherHash(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleBytes);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleBytes, 2);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashByStreamWithStretching_WillReturnAnotherHash(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleStream);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleStream, 2);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashWithSalt_WillReturnAnotherHash(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleBytes);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleBytes, exampleSalt);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashByStreamWithSalt_WillReturnAnotherHash(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleStream);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleStream, exampleSalt);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashByBytesAndStream_ReturnHashIsEqual(IHashCalculator hashCalculator, string className)
    {
        byte[] hash = hashCalculator.ComputeHash(exampleBytes);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleStream);
        Assert.Equal(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashByBytesAndStreamWithStretch_ReturnHashIsEqual(IHashCalculator hashCalculator, string className)
    {
        int stretchCount = 10;
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, stretchCount);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleStream, stretchCount);
        Assert.Equal(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IHashCalculatorObjects))]
    public void ComputeHashByBytesAndStreamWithStretchAndSalt_ReturnHashIsEqual(IHashCalculator hashCalculator, string className)
    {
        int stretchCount = 10;
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, exampleSalt, stretchCount);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleStream, exampleSalt, stretchCount);
        Assert.Equal(hash, hashAnother);
    }
}
