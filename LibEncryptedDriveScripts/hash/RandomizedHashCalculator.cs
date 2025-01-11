namespace LibEncryptedDriveScripts.HashCalculator;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;
using LibEncryptedDriveScripts.KeyGenerator;

public class RandomizedHashCalculator : HashCalculatorBase, IHashCalculator
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
    protected int _seed=0;
    public RandomizedHashCalculator(int seed) : base()
    {
        _seed = seed;
        _algorithmEnumrator = _algorithmSequentialList.GetEnumerator();
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
}
