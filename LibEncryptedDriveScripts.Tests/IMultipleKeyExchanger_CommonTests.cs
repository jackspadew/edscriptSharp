namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.EdData;

#pragma warning disable xUnit1026 // Unused arguments

public class IMultipleKeyExchanger_CommonTests
{
    public class MultipleKeyExchangerBase_Concrete : MultipleKeyExchangerBase {}
    public static IEnumerable<object[]> IMultipleKeyExchangerObjects()
    {
        yield return new object[] { new MultipleKeyExchangerBase_Concrete(), "MultipleKeyExchangerBase_Concrete" };
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void ConvertLikeRoundTrip_ReturnSameBytes(IMultipleKeyExchanger multiKey, string className)
    {
        Random random = new Random(0);
        byte[] sourceBytes = new byte[MultipleKeyExchangerBase.BytesLength];
        random.NextBytes(sourceBytes);
        multiKey.SetBytes(sourceBytes);
        byte[] exchangedBytes = multiKey.GetBytes();
        Assert.Equal(sourceBytes.Length, exchangedBytes.Length);
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
        var targetMultiKey = new MultipleKeyExchangerBase_Concrete();
        multiKey.CopyTo(targetMultiKey);
        Assert.Equal(multiKey.KeySeed, targetMultiKey.KeySeed);
        Assert.Equal(multiKey.IVSeed, targetMultiKey.IVSeed);
        Assert.Equal(multiKey.AlgorithmSeed, targetMultiKey.AlgorithmSeed);
        Assert.Equal(multiKey.HashSeed, targetMultiKey.HashSeed);
        Assert.Equal(multiKey.Key, targetMultiKey.Key);
        Assert.Equal(multiKey.IV, targetMultiKey.IV);
    }
}
