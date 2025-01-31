namespace LibEd.EdData;

using System.IO;
using LibEd.HashCalculator;

public class EdDataHashCalculator : IEdDataHashCalculator
{
    protected int StretchingCount = 1000;
    public byte[] ComputeHash(byte[] inputBytes, IMultipleKeyExchanger multiKey)
    {
        var hashCalculator = CreateHashCalculator(multiKey);
        return hashCalculator.ComputeHash(inputBytes, multiKey.Salt, StretchingCount);
    }
    public byte[] ComputeHash(Stream inputStream, IMultipleKeyExchanger multiKey)
    {
        var hashCalculator = CreateHashCalculator(multiKey);
        return hashCalculator.ComputeHash(inputStream, multiKey.Salt, StretchingCount);
    }
    private IHashCalculator CreateHashCalculator(IMultipleKeyExchanger multiKey)
    {
        return new RandomizedHashCalculator(multiKey.Lye, multiKey.HashSeed);
    }
}
