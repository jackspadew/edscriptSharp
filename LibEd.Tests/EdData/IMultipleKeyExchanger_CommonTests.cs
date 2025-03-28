namespace LibEd.Tests;

using Xunit;
using LibEd.EdData;

#pragma warning disable xUnit1026 // Unused arguments

public class IMultipleKeyExchanger_CommonTests
{
    public class MultipleKeyExchangerBase_Concrete : ExemplaryMultipleKeyExchangerBase {}
    public static IEnumerable<object[]> IMultipleKeyExchangerObjects()
    {
        yield return new object[] { new MultipleKeyExchangerBase_Concrete(), "MultipleKeyExchangerBase_Concrete" };
        yield return new object[] { new DefaultMultipleKeyExchanger(), "DefaultMultipleKeyExchanger" };
        yield return new object[] { new BasicExemplaryMultipleKeyExchanger(), "BasicExemplaryMultipleKeyExchanger" };
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void ConvertLikeRoundTrip_ReturnSameBytes(IMultipleKeyExchanger multiKey, string className)
    {
        Random random = new Random(0);
        int bytesLength = multiKey.GetBytes().Length;
        byte[] sourceBytes = new byte[bytesLength];
        random.NextBytes(sourceBytes);
        multiKey.SetBytes(sourceBytes);
        byte[] exchangedBytes = multiKey.GetBytes();
        Assert.Equal(sourceBytes, exchangedBytes);
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void CopyToLikeRoundTrip_AllValueIsEqual(IMultipleKeyExchanger multiKey, string className)
    {
        Random random = new Random(0);
        multiKey.KeySeed = random.Next();
        multiKey.IVSeed = random.Next();
        multiKey.AlgorithmSeed = random.Next();
        multiKey.HashSeed = random.Next();
        random.NextBytes(multiKey.Key);
        random.NextBytes(multiKey.IV);
        random.NextBytes(multiKey.Salt);
        random.NextBytes(multiKey.Lye);
        var targetMultiKey = new MultipleKeyExchangerBase_Concrete();
        multiKey.CopyTo(targetMultiKey);
        Assert.Equal(multiKey.KeySeed, targetMultiKey.KeySeed);
        Assert.Equal(multiKey.IVSeed, targetMultiKey.IVSeed);
        Assert.Equal(multiKey.AlgorithmSeed, targetMultiKey.AlgorithmSeed);
        Assert.Equal(multiKey.HashSeed, targetMultiKey.HashSeed);
        Assert.Equal(multiKey.Key, targetMultiKey.Key);
        Assert.Equal(multiKey.IV, targetMultiKey.IV);
        Assert.Equal(multiKey.Salt, targetMultiKey.Salt);
        Assert.Equal(multiKey.Lye, targetMultiKey.Lye);
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void Randomized_AllValueIsNotEqual(IMultipleKeyExchanger multiKey, string className)
    {
        var oldKeySeed = multiKey.KeySeed;
        var oldIVSeed = multiKey.IVSeed;
        var oldAlgoSeed = multiKey.AlgorithmSeed;
        var oldHashSeed = multiKey.HashSeed;
        var oldKey = multiKey.Key.Clone();
        var oldIV = multiKey.IV.Clone();
        var oldSalt = multiKey.Salt.Clone();
        var oldLye = multiKey.Lye.Clone();
        multiKey.Randomize();
        Assert.NotEqual(multiKey.KeySeed, oldKeySeed);
        Assert.NotEqual(multiKey.IVSeed, oldIVSeed);
        Assert.NotEqual(multiKey.AlgorithmSeed, oldAlgoSeed);
        Assert.NotEqual(multiKey.HashSeed, oldHashSeed);
        Assert.NotEqual(multiKey.Key, oldKey);
        Assert.NotEqual(multiKey.IV, oldIV);
        Assert.NotEqual(multiKey.Salt, oldSalt);
        Assert.NotEqual(multiKey.Lye, oldLye);
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void Randomized_Int32ValuesCanBeNegativeNumbers(IMultipleKeyExchanger multiKey, string className)
    {
        int tryCount = 10000;
        int negativeCountForKeySeed = 0;
        int negativeCountForIVSeed = 0;
        int negativeCountForAlgoSeed = 0;
        int negativeCountForHashSeed = 0;
        for(int i=0; i<tryCount; i++)
        {
            multiKey.Randomize();
            if(multiKey.KeySeed < 0) negativeCountForKeySeed++;
            if(multiKey.IVSeed < 0) negativeCountForIVSeed++;
            if(multiKey.AlgorithmSeed < 0) negativeCountForAlgoSeed++;
            if(multiKey.HashSeed < 0) negativeCountForHashSeed++;
        }
        double expectedProbability = 0.5;
        double tolerance = 0.05;
        double lowerProbability = expectedProbability - tolerance;
        double upperProbability = expectedProbability + tolerance;
        Assert.InRange((double)negativeCountForKeySeed/tryCount, lowerProbability, upperProbability);
        Assert.InRange((double)negativeCountForIVSeed/tryCount, lowerProbability, upperProbability);
        Assert.InRange((double)negativeCountForAlgoSeed/tryCount, lowerProbability, upperProbability);
        Assert.InRange((double)negativeCountForHashSeed/tryCount, lowerProbability, upperProbability);
    }
}
