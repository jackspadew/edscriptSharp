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
    public RandomizedMultipleEncrypter(int keySeed, int ivSeed)
    {
        _keySeed = keySeed;
        _ivSeed = ivSeed;
    }

    protected override List<byte[]> GenerateKeyList(byte[] key, int count)
    {
        return GenerateRandomBlendedSequentialBytesList(key,count,_keySeed);
    }
    protected override List<byte[]> GenerateIVList(byte[] iv, int count)
    {
        return GenerateRandomBlendedSequentialBytesList(iv,count,_ivSeed);
    }
    private List<byte[]> GenerateRandomBlendedSequentialBytesList(byte[] key, int count, int seed)
    {
        Random random = new Random(_keySeed);
        var converter = new RandomBlendConverter(random);
        var listGenerator = new SequentialGenerator<byte[]>(converter, key);
        return listGenerator.Generate(count);
    }
}
