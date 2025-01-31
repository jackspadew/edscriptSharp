namespace LibEd.EdData;

public interface IEdDataHashCalculator
{
    byte[] ComputeHash(byte[] inputBytes, IMultipleKeyExchanger multiKey);
    byte[] ComputeHash(Stream inputStream, IMultipleKeyExchanger multiKey);
}
