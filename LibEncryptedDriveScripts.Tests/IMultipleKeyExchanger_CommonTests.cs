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
}
