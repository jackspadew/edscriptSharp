namespace LibEncryptedDriveScripts.KeyGenerator;

public class RandomPickedListGenerator<T> : IListGenerator<T>
{
    private int _seed=0;
    private List<T> _availableElementList;
    public RandomPickedListGenerator(List<T> availableElementList, int seed)
    {
        _seed = seed;
        _availableElementList = availableElementList;
    }
    public RandomPickedListGenerator(List<T> availableElementList)
    {
        Random random = new Random();
        _seed = random.Next();
        _availableElementList = availableElementList;
    }
    public virtual List<T> Generate(int length)
    {
        return CreateRandomizedPickedList(length);
    }
    private List<T> CreateRandomizedPickedList(int count)
    {
        List<T> resultList = new();
        IEnumerator<T> randomEnum = new RandomEnumerator<T>(_availableElementList, _seed);
        for(int i=0; i<count; i++)
        {
            resultList.Add(randomEnum.Current);
            randomEnum.MoveNext();
        }
        return resultList;
    }
}
