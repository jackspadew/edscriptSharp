namespace LibEd.Converter;

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

public class BytesListToCombinedBytesConverter : IConverter<List<byte[]>, byte[]>
{
    public byte[] Convert(List<byte[]> input)
    {
        return BytesListToCombinedBytes(input);
    }

    private byte[] BytesListToCombinedBytes(List<byte[]> bytesList)
    {
        List<byte> combinedList = new List<byte>();
        foreach (var byteArray in bytesList)
        {
            combinedList.AddRange(byteArray);
        }
        byte[] result = combinedList.ToArray();
        return result;
    }
}

public class BytesXorBlendConverter : IConverter<byte[], byte[]>
{
    private byte[] _additiveBytes;
    public BytesXorBlendConverter(byte[] additiveBytes)
    {
        _additiveBytes = (byte[])additiveBytes.Clone();
    }

    public byte[] Convert(byte[] input)
    {
        int chunkLength = input.Length;
        byte[] result = (byte[])input.Clone();
        int chunkCount = (_additiveBytes.Length + chunkLength - 1) / chunkLength;
        for (int i=0; i < chunkCount; i++)
        {
            byte[] chunk = new byte[chunkLength];
            int readableChunkSize = Math.Min(chunkLength, _additiveBytes.Length - i * chunkLength);
            Array.Copy(_additiveBytes, i * chunkLength, chunk, 0, readableChunkSize);
            for (int j = 0; j < chunkLength; j++)
            {
                result[j] ^= chunk[j];
            }
        }
        return result;
    }
}
