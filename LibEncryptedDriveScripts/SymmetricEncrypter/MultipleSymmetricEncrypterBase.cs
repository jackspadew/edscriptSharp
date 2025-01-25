namespace LibEncryptedDriveScripts.SymmetricEncrypter;

using LibEncryptedDriveScripts.Converter;
using LibEncryptedDriveScripts.KeyGenerator;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

public abstract class MultipleSymmetricEncrypterBase : SymmetricEncrypterBase, ISymmetricEncrypter
{
    protected abstract int MultipleCryptionCount {get;}
    protected abstract List<ISymmetricAlgorithmAdapter> AlgorithmCandidateList {get;}
    private IEnumerator<ISymmetricAlgorithmAdapter>? _algorithmEnum;
    protected override ISymmetricAlgorithmAdapter Algorithm => _algorithmEnum?.Current ?? throw new NullReferenceException();

    protected byte[] MultipleCryptionWithEnumKeys(Func<byte[], byte[], byte[], byte[]> cryptionFunc, byte[] inputBytes, IEnumerator<ISymmetricAlgorithmAdapter> algorithmEnum, IEnumerator<byte[]> keyEnum, IEnumerator<byte[]> ivEnum)
    {
        byte[] tmpBytes = inputBytes;
        _algorithmEnum = algorithmEnum;
        for(int i=0; i<MultipleCryptionCount; i++)
        {
            if( !algorithmEnum.MoveNext() ) throw new ArgumentOutOfRangeException($"The enumerator \"{nameof(algorithmEnum)}\" has no more elements to retrieve. Cryption number is {i}.");
            if( !keyEnum.MoveNext() ) throw new ArgumentOutOfRangeException($"The enumerator \"{nameof(keyEnum)}\" has no more elements to retrieve. Cryption number is {i}.");
            if( !ivEnum.MoveNext() ) throw new ArgumentOutOfRangeException($"The enumerator \"{nameof(ivEnum)}\" has no more elements to retrieve. Cryption number is {i}.");
            tmpBytes = cryptionFunc(tmpBytes, keyEnum.Current, ivEnum.Current);
        }
        return tmpBytes;
    }
    public override byte[] Encrypt(byte[] inputBytes, byte[] key, byte[] iv)
    {
        var algorithmEnum = GenerateAlgorithmList(MultipleCryptionCount).AsEnumerable().GetEnumerator();
        var keyEnum = GenerateKeyList(key, MultipleCryptionCount).AsEnumerable().GetEnumerator();
        var ivEnum = GenerateIVList(iv, MultipleCryptionCount).AsEnumerable().GetEnumerator();
        byte[] encryptedBytes = MultipleCryptionWithEnumKeys(base.Encrypt, inputBytes, algorithmEnum, keyEnum, ivEnum);
        keyEnum.Dispose();
        ivEnum.Dispose();
        return encryptedBytes;
    }
    public override byte[] Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        var algorithmEnum = GenerateAlgorithmList(MultipleCryptionCount).AsEnumerable().Reverse().GetEnumerator();
        var keyEnum = GenerateKeyList(key, MultipleCryptionCount).AsEnumerable().Reverse().GetEnumerator();
        var ivEnum = GenerateIVList(iv, MultipleCryptionCount).AsEnumerable().Reverse().GetEnumerator();
        byte[] decryptedBytes = MultipleCryptionWithEnumKeys(base.Decrypt, encryptedBytes, algorithmEnum, keyEnum, ivEnum);
        keyEnum.Dispose();
        ivEnum.Dispose();
        return decryptedBytes;
    }
    protected virtual void DisposeCreatedStreams(List<Stream> streamList)
    {
        streamList[0].Dispose();
    }
    protected Stream CallCreateWritableEncryptStreamWithDynamicAlgorithm(ISymmetricAlgorithmAdapter algorithm, Stream outputStream, byte[] key, byte[] iv)
    {
        return algorithm.CreateWritableEncryptStream(outputStream, key, iv);
    }
    protected Stream CallCreateWritableDecryptStreamWithDynamicAlgorithm(ISymmetricAlgorithmAdapter algorithm, Stream outputStream, byte[] key, byte[] iv)
    {
        return algorithm.CreateWritableDecryptStream(outputStream, key, iv);
    }
    protected void MultipleCryptionWithEnumKeys(Func<ISymmetricAlgorithmAdapter, Stream, byte[], byte[], Stream> createWriteableCryptStreamFunc, Stream inputStream, Stream outputStream, IEnumerator<ISymmetricAlgorithmAdapter> algorithmEnum, IEnumerator<byte[]> keyEnum, IEnumerator<byte[]> ivEnum)
    {
        List<Stream> createdStreamList = new();
        Stream nextOutputStream = outputStream;
        for(int i=0; i<MultipleCryptionCount; i++)
        {
            if( !algorithmEnum.MoveNext() ) throw new ArgumentOutOfRangeException($"The enumerator \"{nameof(algorithmEnum)}\" has no more elements to retrieve. Cryption number is {i}.");
            if( !keyEnum.MoveNext() ) throw new ArgumentOutOfRangeException($"The enumerator \"{nameof(keyEnum)}\" has no more elements to retrieve. Cryption number is {i}.");
            if( !ivEnum.MoveNext() ) throw new ArgumentOutOfRangeException($"The enumerator \"{nameof(ivEnum)}\" has no more elements to retrieve. Cryption number is {i}.");
            nextOutputStream = createWriteableCryptStreamFunc(algorithmEnum.Current, nextOutputStream, keyEnum.Current, ivEnum.Current);
            createdStreamList.Insert(0, nextOutputStream);
        }
        Stream FirstEncryptStream = nextOutputStream;
        inputStream.CopyTo(FirstEncryptStream);
        DisposeCreatedStreams(createdStreamList);
    }
    public override void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        var algorithmEnum = GenerateAlgorithmList(MultipleCryptionCount).AsEnumerable().Reverse().GetEnumerator();
        var keyEnum = GenerateKeyList(key, MultipleCryptionCount).AsEnumerable().Reverse().GetEnumerator();
        var ivEnum = GenerateIVList(iv, MultipleCryptionCount).AsEnumerable().Reverse().GetEnumerator();
        MultipleCryptionWithEnumKeys(CallCreateWritableEncryptStreamWithDynamicAlgorithm, inputStream, outputStream, algorithmEnum, keyEnum, ivEnum);
        keyEnum.Dispose();
        ivEnum.Dispose();
    }
    public override void Decrypt(Stream encryptedStream, Stream outputStream, byte[] key, byte[] iv)
    {
        var algorithmEnum = GenerateAlgorithmList(MultipleCryptionCount).AsEnumerable().GetEnumerator();
        var keyEnum = GenerateKeyList(key, MultipleCryptionCount).AsEnumerable().GetEnumerator();
        var ivEnum = GenerateIVList(iv, MultipleCryptionCount).AsEnumerable().GetEnumerator();
        MultipleCryptionWithEnumKeys(CallCreateWritableDecryptStreamWithDynamicAlgorithm, encryptedStream, outputStream, algorithmEnum, keyEnum, ivEnum);
        keyEnum.Dispose();
        ivEnum.Dispose();
    }
    protected virtual List<byte[]> GenerateKeyList(byte[] key, int count)
    {
        return GenerateSameElementList(key,count);
    }
    protected virtual List<byte[]> GenerateIVList(byte[] iv, int count)
    {
        return GenerateSameElementList(iv,count);
    }
    protected virtual List<ISymmetricAlgorithmAdapter> GenerateAlgorithmList(int count)
    {
        return GenerateSameElementList(AlgorithmCandidateList[0],count);
    }
    private List<T> GenerateSameElementList<T>(T element, int count)
    {
        IListGenerator<T> generator = new SameElementListGenerator<T>(element);
        List<T> list = generator.Generate(count);
        return list;
    }
}

public class SameElementListGenerator<T> : SequentialGenerator<T>, IListGenerator<T>
{
    public SameElementListGenerator(T element) : base(new JustReturnConverter<T>(), element)
    {}
}
