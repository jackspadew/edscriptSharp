namespace LibEncryptedDriveScripts.Tests;

using Xunit;
using LibEncryptedDriveScripts.Converter;

public class BytesXorBlendConverter_Tests
{
    public static IEnumerable<object[]> RandomSeeds()
    {
        for (int i = 0; i <= 1000; i++)
        {
            yield return new object[] { i };
        }
    }

    [Fact]
    public void BlendSameBytesWhereAllBitsAreLow_ReturnValueIsCorrect()
    {
        byte[] originalBytes = new byte[]{0,0,0,0};
        byte[] additiveBytes = new byte[]{0,0,0,0};
        var converter = new BytesXorBlendConverter(additiveBytes);
        byte[] result = converter.Convert(originalBytes);
        byte[] expectedBytes = new byte[]{0,0,0,0};
        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public void BlendSameBytesWhereAllBitsAreHigh_ReturnValueIsCorrect()
    {
        byte[] originalBytes = new byte[]{255,255,255,255};
        byte[] additiveBytes = new byte[]{255,255,255,255};
        var converter = new BytesXorBlendConverter(additiveBytes);
        byte[] result = converter.Convert(originalBytes);
        byte[] expectedBytes = new byte[]{0,0,0,0};
        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public void BlendWithAdditiveByteIsLongLength_ShortageChunkWillFilledLowThenXOR()
    {
        byte[] originalBytes = new byte[]{255,255,255,255};
        byte[] additiveBytes = new byte[]{255,255,255,255,255,255};
        var converter = new BytesXorBlendConverter(additiveBytes);
        byte[] result = converter.Convert(originalBytes);
        byte[] expectedBytes = new byte[]{255,255,0,0};
        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public void BlendWithAdditiveByteIsShortLength_ShortageChunkWillFilledLowThenXOR()
    {
        byte[] originalBytes = new byte[]{255,255,255,255};
        byte[] additiveBytes = new byte[]{255,255};
        var converter = new BytesXorBlendConverter(additiveBytes);
        byte[] result = converter.Convert(originalBytes);
        byte[] expectedBytes = new byte[]{0,0,255,255};
        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public void BlendWithDoubleLengthBytes_ReturnValueIsCorrect()
    {
        byte[] originalBytes = new byte[]{255,255,255,255};
        byte[] additiveBytes = new byte[]{255,255,255,255,255,255,255,255};
        var converter = new BytesXorBlendConverter(additiveBytes);
        byte[] result = converter.Convert(originalBytes);
        byte[] expectedBytes = new byte[]{255,255,255,255};
        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public void BlendManyRandomCombinations_ReturnValueIsCorrect()
    {
        Random random = new Random(0);
        for(int i=0; i < 100000; i++)
        {
            int valueOne = random.Next();
            int valueTwo = random.Next();
            var bytesOne = BitConverter.GetBytes(valueOne);
            var bytesTwo = BitConverter.GetBytes(valueTwo);
            var converter = new BytesXorBlendConverter(bytesTwo);
            byte[] resultBytes = converter.Convert(bytesOne);
            int result = BitConverter.ToInt32(resultBytes);
            int expected = valueOne ^ valueTwo;
            Assert.Equal(expected, result);
        }
    }
}
