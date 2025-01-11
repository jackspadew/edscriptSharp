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
        Random random = new Random(_seed);
        return Enumerable.Range(1, count)
                         .Select(_ => _availableElementList[random.Next(_availableElementList.Count)])
                         .ToList();
    }
}
