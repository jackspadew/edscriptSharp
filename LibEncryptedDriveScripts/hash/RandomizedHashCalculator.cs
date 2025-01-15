namespace LibEncryptedDriveScripts.HashCalculator;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;
using LibEncryptedDriveScripts.KeyGenerator;
using LibEncryptedDriveScripts.Converter;

public class RandomizedHashCalculator : LyeHashCalculatorBase, IHashCalculator
{
    protected static List<IHashAlgorithmAdapter> AvailableAlgorithmList = new(){
        new HashAlgorithmAdapter.BouncyCastle.SHA3(),
        };
    private IEnumerator<IHashAlgorithmAdapter> _algorithmEnumrator;
    private List<IHashAlgorithmAdapter> _algorithmSequentialList = new();
    protected override IHashAlgorithmAdapter Algorithm {
        get {
            var value = _algorithmEnumrator.Current;
            _algorithmEnumrator.MoveNext();
            return value;
        }
        }
    private byte[] _lyeOriginBytes = new byte[0];
    private IEnumerator<byte[]> _lyeBytesEnumrator;
    private List<byte[]> _lyeBytesList = new();
    protected override byte[] LyeBytes {
        get
        {
            if(_lyeBytesList.Count == 0) return _lyeOriginBytes;
            var value = _lyeBytesEnumrator.Current;
            _lyeBytesEnumrator.MoveNext();
            return value;
        }
        set
        {
            _lyeOriginBytes = value;
        }
        }
    protected int _seed=0;
    public RandomizedHashCalculator(int seed) : this(new byte[0], seed)
    {}
    public RandomizedHashCalculator(byte[] lyeBytes, int seed)
    {
        _seed = seed;
        _algorithmEnumrator = _algorithmSequentialList.GetEnumerator();
        _lyeBytesEnumrator = _lyeBytesList.GetEnumerator();
        _lyeOriginBytes = lyeBytes;
    }
    public override byte[] ComputeHash(byte[] inputBytes, int stretchCount = 1)
    {
        ResetAlgorithmSequentialList(stretchCount);
        return base.ComputeHash(inputBytes, stretchCount);
    }
    public override byte[] ComputeHash(Stream inputStream, int stretchCount = 1)
    {
        ResetAlgorithmSequentialList(stretchCount);
        return base.ComputeHash(inputStream, stretchCount);
    }
    private void ResetAlgorithmSequentialList(int length)
    {
        var generator = new RandomPickedListGenerator<IHashAlgorithmAdapter>(AvailableAlgorithmList, _seed);
        _algorithmSequentialList = generator.Generate(length);
        _algorithmEnumrator = _algorithmSequentialList.GetEnumerator();
        _algorithmEnumrator.MoveNext();
    }
    private void GenerateLyeSequentialList(int length)
    {
        Random random = new Random(_seed);
        byte[] initialLye = _lyeOriginBytes;
        var converter = new RandomBlendConverter(random);
        var generator = new SequentialGenerator<byte[]>(converter, initialLye);
        _lyeBytesList = generator.Generate(length);
        _lyeBytesEnumrator = _lyeBytesList.GetEnumerator();
        _lyeBytesEnumrator.MoveNext();
    }
    protected override byte[] Stretching(byte[] hashBytes, int stretchCount)
    {
        GenerateLyeSequentialList(stretchCount-1);
        return base.Stretching(hashBytes, stretchCount);
    }
}
