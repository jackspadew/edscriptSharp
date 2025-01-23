namespace LibEncryptedDriveScripts.SymmetricEncrypter;

using LibEncryptedDriveScripts.Converter;
using LibEncryptedDriveScripts.KeyGenerator;
using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

public class RandomizedMultipleEncrypter : SymmetricEncrypterBase, ISymmetricEncrypter
{
    protected int _keySeed;
    protected int _ivSeed;
    private static List<ISymmetricAlgorithmAdapter> AlgorithmCandidateList {get;} = new()
    {
        new SymmetricAlgorithmAdapter.SystemCryptography.AES(),
        new SymmetricAlgorithmAdapter.BouncyCastle.AES(),
        new SymmetricAlgorithmAdapter.BouncyCastle.Camellia(),
        new SymmetricAlgorithmAdapter.BouncyCastle.Serpent(),
        new SymmetricAlgorithmAdapter.BouncyCastle.Twofish(),
    };
    public RandomizedMultipleEncrypter(int keySeed, int ivSeed, int algoSeed, int multiple)
    {
        _keySeed = keySeed;
        _ivSeed = ivSeed;
        _algorithm = CreateSymmetricAlgorithmComboList(algoSeed, multiple);
    }
    public RandomizedMultipleEncrypter() : this(0,0,0,10) {}

    protected override List<byte[]> GenerateKeyList(byte[] key, int count)
    {
        return GenerateRandomBlendedSequentialBytesList(key,count,_keySeed);
    }
    protected override List<byte[]> GenerateIVList(byte[] iv, int count)
    {
        return GenerateRandomBlendedSequentialBytesList(iv,count,_ivSeed);
    }
    private List<byte[]> GenerateRandomBlendedSequentialBytesList(byte[] bytes, int count, int seed)
    {
        Random random = new Random(seed);
        var converter = new RandomBlendConverter(random);
        var listGenerator = new SequentialGenerator<byte[]>(converter, bytes);
        return listGenerator.Generate(count);
    }
    private List<ISymmetricAlgorithmAdapter> CreateSymmetricAlgorithmComboList(int seed, int count)
    {
        var generator = new RandomPickedListGenerator<ISymmetricAlgorithmAdapter>(AlgorithmCandidateList, seed);
        return generator.Generate(count);
    }
}
