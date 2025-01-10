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
        new SymmetricAlgorithmAdapter.BouncyCastle.AES()
    };
    public RandomizedMultipleEncrypter(int keySeed, int ivSeed, int algoSeed, int multiple)
    {
        _keySeed = keySeed;
        _ivSeed = ivSeed;
        _algorithm = CreateSymmetricAlgorithmComboList(AlgorithmCandidateList, algoSeed, multiple);
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
    private List<ISymmetricAlgorithmAdapter> CreateSymmetricAlgorithmComboList(List<ISymmetricAlgorithmAdapter> validAlgorithmList, int seed, int count)
    {
        Random random = new Random(seed);
        return Enumerable.Range(0, count)
                         .Select(_ => validAlgorithmList[random.Next(validAlgorithmList.Count)])
                         .ToList();
    }
}
