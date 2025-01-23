namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.EdData;

#pragma warning disable xUnit1026 // Unused arguments

public class KeyBlendedMultipleKeyExchangerBase_CommonTests
{
    public class Concrete_ExemplaryMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase {}
    public static IEnumerable<object[]> IMultipleKeyExchangerObjects()
    {
        yield return new object[] { new BasicKeyBlendedMultipleKeyExchanger(), "BasicKeyBlendedMultipleKeyExchanger" };
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
    public void ConvertLikeRoundTrip_ReturnSameValues(IMultipleKeyExchanger multiKey, string className)
    {
        multiKey.Randomize();
        var oldKeySeed = multiKey.KeySeed;
        var oldIVSeed = multiKey.IVSeed;
        var oldAlgoSeed = multiKey.AlgorithmSeed;
        var oldHashSeed = multiKey.HashSeed;
        var oldIV = multiKey.IV.Clone();
        var oldSalt = multiKey.Salt.Clone();
        var oldLye = multiKey.Lye.Clone();
        byte[] backupedKey = (byte[])multiKey.Key.Clone();
        byte[] exportedBytes = multiKey.GetBytes();
        multiKey.Randomize();
        multiKey.SetBytes(exportedBytes);
        multiKey.Key = backupedKey;
        Assert.Equal(multiKey.KeySeed, oldKeySeed);
        Assert.Equal(multiKey.IVSeed, oldIVSeed);
        Assert.Equal(multiKey.AlgorithmSeed, oldAlgoSeed);
        Assert.Equal(multiKey.HashSeed, oldHashSeed);
        Assert.Equal(multiKey.IV, oldIV);
        Assert.Equal(multiKey.Salt, oldSalt);
        Assert.Equal(multiKey.Lye, oldLye);
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void SetKey_ProperiesReturnAnotherValue(IMultipleKeyExchanger multiKey, string className)
    {
        Random random = new Random(0);
        byte[] anotherKey = new byte[multiKey.Key.Length];
        random.NextBytes(anotherKey);
        var oldKeySeed = multiKey.KeySeed;
        var oldIVSeed = multiKey.IVSeed;
        var oldAlgoSeed = multiKey.AlgorithmSeed;
        var oldHashSeed = multiKey.HashSeed;
        var oldKey = multiKey.Key.Clone();
        var oldIV = multiKey.IV.Clone();
        var oldSalt = multiKey.Salt.Clone();
        var oldLye = multiKey.Lye.Clone();
        multiKey.Key = anotherKey;
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
    public void GetBytes_DoesNotExportKey(IMultipleKeyExchanger multiKey, string className)
    {
        multiKey.Randomize();
        byte[] backupedKey = (byte[])multiKey.Key.Clone();
        byte[] exportedBytes = multiKey.GetBytes();
        multiKey.Randomize();
        multiKey.SetBytes(exportedBytes);
        Assert.NotEqual(backupedKey, multiKey.Key);
    }

    [Theory]
    [MemberData(nameof(IMultipleKeyExchangerObjects))]
    public void GenerateBlendedKeyFromExemplaryKey_PropertiesAreAnotherValue(IMultipleKeyExchanger multiKey, string className)
    {
        Random random = new Random(0);
        byte[] secretKey = new byte[multiKey.Key.Length];
        random.NextBytes(secretKey);
        var exemplaryKey = new Concrete_ExemplaryMultipleKeyExchanger();
        exemplaryKey.Randomize();
        var oldKeySeed = exemplaryKey.KeySeed;
        var oldIVSeed = exemplaryKey.IVSeed;
        var oldAlgoSeed = exemplaryKey.AlgorithmSeed;
        var oldHashSeed = exemplaryKey.HashSeed;
        var oldIV = exemplaryKey.IV.Clone();
        var oldSalt = exemplaryKey.Salt.Clone();
        var oldLye = exemplaryKey.Lye.Clone();
        exemplaryKey.CopyTo(multiKey);
        multiKey.Key = secretKey;
        Assert.NotEqual(multiKey.KeySeed, oldKeySeed);
        Assert.NotEqual(multiKey.IVSeed, oldIVSeed);
        Assert.NotEqual(multiKey.AlgorithmSeed, oldAlgoSeed);
        Assert.NotEqual(multiKey.HashSeed, oldHashSeed);
        Assert.NotEqual(multiKey.IV, oldIV);
        Assert.NotEqual(multiKey.Salt, oldSalt);
        Assert.NotEqual(multiKey.Lye, oldLye);
    }
}
