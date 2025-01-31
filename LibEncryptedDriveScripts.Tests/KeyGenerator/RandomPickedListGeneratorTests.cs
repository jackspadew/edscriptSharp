namespace LibEd.Tests;

using Xunit;
using LibEd.KeyGenerator;

public class RandomPickedListGenerator_Tests
{
    private int SpecifiedLengthNumber = 1000;

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GenerateUsingIntList_TheRatiosInReturnedListAreIdeal(int seed)
    {
        var exampleList = new List<int>(){0,1,2,3};
        var generator = new RandomPickedListGenerator<int>(exampleList, seed);
        var list = generator.Generate(SpecifiedLengthNumber);
        var previousValue = list[0];
        foreach(var value in exampleList)
        {
            int countOfValue = list.Count(x => x == value);
            double resultRetio = (double)countOfValue / SpecifiedLengthNumber;
            double theoreticalRatio = 1 / (double)exampleList.Count;
            Assert.InRange(resultRetio, theoreticalRatio - 0.05, theoreticalRatio + 0.05);
        }
    }
}
