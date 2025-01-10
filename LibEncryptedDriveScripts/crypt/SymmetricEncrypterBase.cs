namespace LibEncryptedDriveScripts.SymmetricEncrypter;

using LibEncryptedDriveScripts.Converter;
using LibEncryptedDriveScripts.KeyGenerator;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

public abstract class SymmetricEncrypterBase : ISymmetricEncrypter
{
    protected List<ISymmetricAlgorithmAdapter> _algorithm = new();

    public virtual byte[] Encrypt(byte[] inputBytes, byte[] key, byte[] iv)
    {
        byte[] tmpBytes = inputBytes;
        var keyEnum = GenerateKeyList(key, _algorithm.Count).AsEnumerable().GetEnumerator();
        var ivEnum = GenerateIVList(iv, _algorithm.Count).AsEnumerable().GetEnumerator();
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm)
        {
            keyEnum.MoveNext();
            ivEnum.MoveNext();
            tmpBytes = algo.EncryptBytes(tmpBytes, keyEnum.Current, ivEnum.Current);
        }
        keyEnum.Dispose();
        ivEnum.Dispose();
        return tmpBytes;
    }
    public virtual byte[] Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        byte[] tmpBytes = encryptedBytes;
        var keyEnum = GenerateKeyList(key, _algorithm.Count).AsEnumerable().Reverse().GetEnumerator();
        var ivEnum = GenerateIVList(iv, _algorithm.Count).AsEnumerable().Reverse().GetEnumerator();
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm.AsEnumerable().Reverse())
        {
            keyEnum.MoveNext();
            ivEnum.MoveNext();
            tmpBytes = algo.DecryptBytes(tmpBytes, keyEnum.Current, ivEnum.Current);
        }
        keyEnum.Dispose();
        ivEnum.Dispose();
        return tmpBytes;
    }

    protected virtual void DisposeCreatedStreams(List<Stream> streamList)
    {
        foreach(Stream stream in streamList)
        {
            stream.Dispose();
        }
    }
    public virtual void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        List<Stream> createdStreamList = new();
        Stream nextOutputStream = outputStream;
        var keyEnum = GenerateKeyList(key, _algorithm.Count).AsEnumerable().Reverse().GetEnumerator();
        var ivEnum = GenerateIVList(iv, _algorithm.Count).AsEnumerable().Reverse().GetEnumerator();
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm.AsEnumerable().Reverse())
        {
            keyEnum.MoveNext();
            ivEnum.MoveNext();
            nextOutputStream = algo.CreateWritableEncryptStream(nextOutputStream, keyEnum.Current, ivEnum.Current);
            createdStreamList.Insert(0, nextOutputStream);
        }
        keyEnum.Dispose();
        ivEnum.Dispose();
        Stream FirstEncryptStream = nextOutputStream;
        inputStream.CopyTo(FirstEncryptStream);
        DisposeCreatedStreams(createdStreamList);
    }
    public virtual void Decrypt(Stream encryptedStream, Stream outputStream, byte[] key, byte[] iv)
    {
        List<Stream> createdStreamList = new();
        Stream nextOutputStream = outputStream;
        var keyEnum = GenerateKeyList(key, _algorithm.Count).AsEnumerable().GetEnumerator();
        var ivEnum = GenerateIVList(iv, _algorithm.Count).AsEnumerable().GetEnumerator();
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm)
        {
            keyEnum.MoveNext();
            ivEnum.MoveNext();
            nextOutputStream = algo.CreateWritableDecryptStream(nextOutputStream, keyEnum.Current, ivEnum.Current);
            createdStreamList.Insert(0, nextOutputStream);
        }
        keyEnum.Dispose();
        ivEnum.Dispose();
        Stream FirstDecryptStream = nextOutputStream;
        encryptedStream.CopyTo(FirstDecryptStream);
        DisposeCreatedStreams(createdStreamList);
    }
    protected virtual List<byte[]> GenerateKeyList(byte[] key, int count)
    {
        return GenerateSameBytesList(key,count);
    }
    protected virtual List<byte[]> GenerateIVList(byte[] iv, int count)
    {
        return GenerateSameBytesList(iv,count);
    }
    private List<byte[]> GenerateSameBytesList(byte[] initBytes, int count)
    {
        IListGenerator<byte[]> generator = new SameKeyListGenerator(initBytes);
        List<byte[]> list = generator.Generate(count);

        return list;
    }
}

public class SameKeyListGenerator : SequentialGenerator<byte[]>, IListGenerator<byte[]>
{
    public SameKeyListGenerator(byte[] initBytes) : base(new JustReturnConverter<byte[]>(), initBytes)
    {}
}
