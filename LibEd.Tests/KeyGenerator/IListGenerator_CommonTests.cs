namespace LibEd.Tests;

using Xunit;
using LibEd.KeyGenerator;
using LibEd.Converter;

#pragma warning disable xUnit1026 // Unused arguments

public class IListGenerator_CommonTests
{
    private int SpecifiedLengthNumber = 100;
    public static IEnumerable<object[]> IListGeneratorObjects()
    {
        yield return new object[] { new RandomPickedListGenerator<int>(new List<int>(){0,1,2,3}, 0), "RandomPickedListGenerator" };
        yield return new object[] { new SequentialGenerator<int>(new JustReturnConverter<int>(), 0), "SequentialGenerator" };
    }

    [Theory]
    [MemberData(nameof(IListGeneratorObjects))]
    public void GenerateList_ReturnedListLengthIsEqualSpecifiedNumber(IListGenerator<int> generator, string className)
    {
        List<int> result = generator.Generate(100);
        Assert.Equal(SpecifiedLengthNumber, result.Count);
    }
}
