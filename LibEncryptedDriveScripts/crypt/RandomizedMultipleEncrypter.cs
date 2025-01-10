namespace LibEncryptedDriveScripts.SymmetricEncrypter;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

public class RandomizedMultipleEncrypter : SymmetricEncrypterBase, ISymmetricEncrypter
{
    protected Random _keyRandom;
    protected Random _ivRandom;
    private static List<ISymmetricAlgorithmAdapter> AlgorithmCandidateList {get;} = new()
    {
        new SymmetricAlgorithmAdapter.SystemCryptography.AES(),
        new SymmetricAlgorithmAdapter.BouncyCastle.AES()
    };
    public RandomizedMultipleEncrypter(int keySeed, int ivSeed)
    {
        _keyRandom = new Random(keySeed);
        _ivRandom = new Random(ivSeed);
    }
}
