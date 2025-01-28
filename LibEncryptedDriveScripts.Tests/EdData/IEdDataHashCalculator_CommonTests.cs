namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;

#pragma warning disable xUnit1026 // Unused arguments

public class IEdDataHashCalculator_CommonTests
{
    private static byte[] exampleBytes = new byte[]{0,1,2,3};
    private static byte[] anotherBytes = new byte[]{4,5,6,7};
    public static IEnumerable<object[]> IEdDataHashCalculator_Objects()
    {
        yield return new object[] { new EdDataHashCalculator(), nameof(EdDataHashCalculator) };
    }

    private IMultipleKeyExchanger GetRandomizedMultipleKey()
    {
        var multiKey = new BasicExemplaryMultipleKeyExchanger();
        multiKey.Randomize();
        return multiKey;
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashByBytes_ReturnLengthIsEqual512Bits(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, multiKey);
        Assert.Equal(512, hash.Length * 8);
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashByStream_ReturnLengthIsEqual512Bits(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        var stream = new MemoryStream(exampleBytes);
        byte[] hash = hashCalculator.ComputeHash(stream, multiKey);
        Assert.Equal(512, hash.Length * 8);
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashByBytesAndStream_ReturnSameBytes(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        var stream = new MemoryStream(exampleBytes);
        byte[] hashByBytes = hashCalculator.ComputeHash(exampleBytes, multiKey);
        byte[] hashByStream = hashCalculator.ComputeHash(stream, multiKey);
        Assert.Equal(hashByBytes, hashByStream);
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashAnotherBytes_ReturnAnotherBytes(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, multiKey);
        byte[] hashAnother = hashCalculator.ComputeHash(anotherBytes, multiKey);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashWithCopiedMultipleKey_ReturnSameBytes(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        IMultipleKeyExchanger anotherMultiKey = new BasicExemplaryMultipleKeyExchanger();
        multiKey.CopyTo(anotherMultiKey);
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, multiKey);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleBytes, anotherMultiKey);
        Assert.Equal(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashWithAnotherSaltMultipleKey_ReturnAnotherBytes(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        IMultipleKeyExchanger anotherMultiKey = new BasicExemplaryMultipleKeyExchanger();
        multiKey.CopyTo(anotherMultiKey);
        anotherMultiKey.Salt[0] += 1;
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, multiKey);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleBytes, anotherMultiKey);
        Assert.NotEqual(hash, hashAnother);
    }

    [Theory]
    [MemberData(nameof(IEdDataHashCalculator_Objects))]
    public void ComputeHashWithAnotherLyeMultipleKey_ReturnAnotherBytes(IEdDataHashCalculator hashCalculator, string className)
    {
        IMultipleKeyExchanger multiKey = GetRandomizedMultipleKey();
        IMultipleKeyExchanger anotherMultiKey = new BasicExemplaryMultipleKeyExchanger();
        multiKey.CopyTo(anotherMultiKey);
        anotherMultiKey.Lye[0] += 1;
        byte[] hash = hashCalculator.ComputeHash(exampleBytes, multiKey);
        byte[] hashAnother = hashCalculator.ComputeHash(exampleBytes, anotherMultiKey);
        Assert.NotEqual(hash, hashAnother);
    }
}
