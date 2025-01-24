namespace LibEncryptedDriveScripts.Tests;

using LibEncryptedDriveScripts.KeyGenerator;

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
}
