namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.EdData;

public class StringToHashConverter_Tests
{
    private static string exampleString = "example";
    private static string anotherString = "another";
    private IEdDataHashCalculator HashCalculator => new EdDataHashCalculator();
    private IMultipleKeyExchanger MultipleKey => new BasicExemplaryMultipleKeyExchanger();

    [Fact]
    public void Convert_ReturnHasEnoughLength()
    {
        var hashCalculator = this.HashCalculator;
        var multiKey = this.MultipleKey;
        var converter = new StringToHashConverter(hashCalculator, multiKey);
        var hash = converter.Convert(exampleString);
        Assert.InRange(hash.Length, 32, Int32.MaxValue);
    }

    [Fact]
    public void ConvertTwice_ReturnSame()
    {
        var hashCalculator = this.HashCalculator;
        var multiKey = this.MultipleKey;
        var converter = new StringToHashConverter(hashCalculator, multiKey);
        var hash = converter.Convert(exampleString);
        var hash2nd = converter.Convert(exampleString);
        Assert.Equal(hash, hash2nd);
    }

    [Fact]
    public void ConvertAnotherString_ReturnAnotherHash()
    {
        var hashCalculator = this.HashCalculator;
        var multiKey = this.MultipleKey;
        var converter = new StringToHashConverter(hashCalculator, multiKey);
        var hash = converter.Convert(exampleString);
        var hashAnother = converter.Convert(anotherString);
        Assert.NotEqual(hash, hashAnother);
    }

    [Fact]
    public void ConvertAnotherMultipleKey_ReturnAnotherHash()
    {
        var hashCalculator = this.HashCalculator;
        var multiKey = this.MultipleKey;
        var multiKeyAnother = this.MultipleKey;
        multiKey.Randomize();
        multiKeyAnother.Randomize();
        var converter = new StringToHashConverter(hashCalculator, multiKey);
        var converterAnother = new StringToHashConverter(hashCalculator, multiKeyAnother);
        var hash = converter.Convert(exampleString);
        var hashAnother = converterAnother.Convert(anotherString);
        Assert.NotEqual(hash, hashAnother);
    }
}
