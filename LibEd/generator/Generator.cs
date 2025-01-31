namespace LibEd.KeyGenerator;

using LibEd.Converter;

public class SequentialGenerator<T> : IListGenerator<T>
{
    protected T _initialValue;
    private IConverter<T,T> _converter;
    public SequentialGenerator(IConverter<T,T> converter, T initValue)
    {
        _converter = converter;
        _initialValue = initValue;
    }
    public virtual List<T> Generate(int length)
    {
        List<T> resultList = new();
        T tmpValue = _initialValue;

        for(int i=0; i < length; i++)
        {
            tmpValue = _converter.Convert(tmpValue);
            resultList.Add(tmpValue);
        }
        return resultList;
    }
}

public class SameElementListGenerator<T> : SequentialGenerator<T>, IListGenerator<T>
{
    public SameElementListGenerator(T element) : base(new JustReturnConverter<T>(), element)
    {}
}
