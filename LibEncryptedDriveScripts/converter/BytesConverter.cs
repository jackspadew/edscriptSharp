namespace LibEncryptedDriveScripts.Converter;

public class JustReturnConverter<T> : IConverter<T, T>
{
    public T Convert(T input)
    {
        return input;
    }
}

public class RandomBlendConverter : IConverter<byte[], byte[]>
{
    protected Random _random;

    public RandomBlendConverter(Random random)
    {
        _random = random;
    }

    public byte[] Convert(byte[] input)
    {
        byte[] buffer = new byte[input.Length];
        _random.NextBytes(buffer);
        for(int i=0; i < input.Length; i++)
        {
            buffer[i] = (byte)(buffer[i] ^ input[i]);
        }
        return buffer;
    }
}
