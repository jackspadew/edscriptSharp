namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.HashCalculator;

public class RandomizedHashCalculator_Tests
{
    public class BasicHashCalculator : HashCalculatorBase
    {}
    private int StandardStretchingCount = 1000;
    private byte[] exampleBytes = new byte[]{0,1,2,3};
    private byte[] exampleSalt = new byte[]{1,1};
    private byte[] exampleLye = new byte[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15};

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void GenerateHashWithStretching_NotThrowNotEmpty(int stretchCount)
    {
        var randomizedHashCalculator = new RandomizedHashCalculator(0);
        byte[] hash = randomizedHashCalculator.ComputeHash(exampleBytes, stretchCount);
        Assert.NotInRange(hash.Length, 0, 31);
    }

    [Theory(Skip = "Algorithm randomizers do not work because there is only one algorithm employed.")]
    [InlineData(0,1)]
    [InlineData(1,2)]
    [InlineData(2,3)]
    [InlineData(3,4)]
    [InlineData(4,5)]
    public void GenerateHashAnotherSeed_ReturnAnotherHash(int seedOne, int seedTwo)
    {
        var calculatorOne = new RandomizedHashCalculator(seedOne);
        var calculatorTwo = new RandomizedHashCalculator(seedTwo);
        byte[] hashOne = calculatorOne.ComputeHash(exampleBytes, StandardStretchingCount);
        byte[] hashTwo = calculatorTwo.ComputeHash(exampleBytes, StandardStretchingCount);
        Assert.NotEqual(hashOne, hashTwo);
    }

    [Theory]
    [InlineData(0,1)]
    [InlineData(1,2)]
    [InlineData(2,3)]
    [InlineData(3,4)]
    [InlineData(4,5)]
    public void GenerateHashWithLye_ReturnAnotherHash(int lyeOne, int lyeTwo)
    {
        int sameSeed = 0;
        byte[] lyeOneBytes = BitConverter.GetBytes(lyeOne);
        byte[] lyeTwoBytes = BitConverter.GetBytes(lyeTwo);
        var calculatorOne = new RandomizedHashCalculator(lyeOneBytes, sameSeed);
        var calculatorTwo = new RandomizedHashCalculator(lyeTwoBytes, sameSeed);
        byte[] hashOne = calculatorOne.ComputeHash(exampleBytes, StandardStretchingCount);
        byte[] hashTwo = calculatorTwo.ComputeHash(exampleBytes, StandardStretchingCount);
        Assert.NotEqual(hashOne, hashTwo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GenerateHashWithoutStretchingByThisAndBasicCalculator_ReturnSameHash(int seed)
    {
        var randomizedHashCalculator = new RandomizedHashCalculator(seed);
        var basicHashCalculator = new BasicHashCalculator();
        byte[] hashRandomized = randomizedHashCalculator.ComputeHash(exampleBytes, 1);
        byte[] hashBasic = basicHashCalculator.ComputeHash(exampleBytes, 1);
        Assert.Equal(hashBasic, hashRandomized);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GenerateHashWithSaltWithoutStretchingByThisAndBasicCalculator_ReturnSameHash(int seed)
    {
        var randomizedHashCalculator = new RandomizedHashCalculator(seed);
        var basicHashCalculator = new BasicHashCalculator();
        byte[] hashRandomized = randomizedHashCalculator.ComputeHash(exampleBytes, exampleSalt, 1);
        byte[] hashBasic = basicHashCalculator.ComputeHash(exampleBytes, exampleSalt, 1);
        Assert.Equal(hashBasic, hashRandomized);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GenerateHashWithStretchingWithoutLye_ReturnSameHash(int seed)
    {
        var randomizedHashCalculator = new RandomizedHashCalculator(seed);
        var basicHashCalculator = new BasicHashCalculator();
        byte[] hashRandomized = randomizedHashCalculator.ComputeHash(exampleBytes, StandardStretchingCount);
        byte[] hashBasic = basicHashCalculator.ComputeHash(exampleBytes, StandardStretchingCount);
        Assert.Equal(hashBasic, hashRandomized);
    }
}
