namespace LibEd.EdData;

using LibEd.Converter;
using System.Text;

public class StringToHashConverter : IConverter<string, byte[]>
{
    protected IEdDataHashCalculator _hashCalculator;
    protected IMultipleKeyExchanger _multipleKey;
    public StringToHashConverter(IEdDataHashCalculator hashCalculator, IMultipleKeyExchanger multipleKey)
    {
        _hashCalculator = hashCalculator;
        _multipleKey = multipleKey;
    }
    public byte[] Convert(string input)
    {
        byte[] stringBytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = _hashCalculator.ComputeHash(stringBytes, _multipleKey);
        return hash;
    }
}
