namespace LibEd.Tests;

using Xunit;
using LibEd.KeyGenerator;
using LibEd.Converter;

public class SequentialGenerator_Tests
{
    [Fact]
    public void UsingWithJustReturnConverter_AllElementsOfReturnListIsEqual()
    {
        int testCount = 1000;
        var exampleValue = new byte[]{0,1,2,3};
        var converter = new JustReturnConverter<byte[]>();
        var generator = new SequentialGenerator<byte[]>(converter, exampleValue);
        var list = generator.Generate(testCount);
        var previousValue = list[0];
        foreach(var value in list[1..])
        {
            Assert.Equal(previousValue, value);
            previousValue = value;
        }
    }

    [Fact]
    public void UsingWithRandomBlendConverter_ReturnListAlmostAllElementsIsMismatch()
    {
        int testCount = 10000;
        Random random = new Random(0);
        var exampleValue = new byte[]{0,1,2,3};
        var converter = new RandomBlendConverter(random);
        var generator = new SequentialGenerator<byte[]>(converter, exampleValue);
        var list = generator.Generate(testCount);
        var firstValue = list[0];
        int wrongCount = 0;
        foreach(var value in list[1..])
        {
            if(!value.SequenceEqual(firstValue)) wrongCount++;
        }
        double mismatchRate = (double)wrongCount / (double)testCount;
        Assert.True( mismatchRate >= 0.999 );
    }
}
