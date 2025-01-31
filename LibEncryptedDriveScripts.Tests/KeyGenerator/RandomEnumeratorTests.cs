namespace LibEd.Tests;

using LibEd.KeyGenerator;

public class RandomEnumerator_Tests
{
    private static List<int> exampleList => new(){0,1,2,3,4,5,6,7,8,9};

    [Fact]
    public void TakeManyCurrentValue_AllValuesAreInList()
    {
        var candidatesList = exampleList;
        var valueList = new List<int>();
        var randomEnum = new RandomEnumerator<int>(candidatesList,0);
        for(int i=0; i<1000; i++)
        {
            valueList.Add(randomEnum.Current);
            randomEnum.MoveNext();
        }
        bool AllValuesAreInlist = valueList.All(a => candidatesList.Contains(a));
        Assert.True(AllValuesAreInlist);
    }

    [Fact]
    public void TakeManyCurrentValue_AllCandidatesAppeared()
    {
        var candidatesList = exampleList;
        var valueList = new List<int>();
        var randomEnum = new RandomEnumerator<int>(candidatesList,0);
        for(int i=0; i<1000; i++)
        {
            valueList.Add(randomEnum.Current);
            randomEnum.MoveNext();
        }
        bool AllValuesAreInlist = candidatesList.All(a => valueList.Contains(a));
        Assert.True(AllValuesAreInlist);
    }

    [Fact]
    public void TakeManyCurrentValue_OccurrenceRateOfAllCandidateValue​​IsWithinAcceptableRange()
    {
        var candidatesList = exampleList;
        var valueList = new List<int>();
        var randomEnum = new RandomEnumerator<int>(candidatesList,0);
        for(int i=0; i<10000; i++)
        {
            valueList.Add(randomEnum.Current);
            randomEnum.MoveNext();
        }
        var frequencies = candidatesList.ToDictionary(b => b, b => valueList.Count(a => a == b));
        bool IsOccurrenceRateWithinAcceptableRange = true;
        double tolerance = 0.1;
        double idealRate = 1 / candidatesList.Count;
        foreach(var keyValuePair in frequencies)
        {
            double actualRate = keyValuePair.Value / valueList.Count;
            double difference = Math.Abs(actualRate - idealRate);
            if(difference > tolerance)
            {
                Assert.Fail($"The rate of value {keyValuePair.Key} was {actualRate} (Difference from ideal rate {difference}).");
            }
        }
        Assert.True(IsOccurrenceRateWithinAcceptableRange);
    }
}
