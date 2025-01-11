namespace LibEncryptedDriveScripts.HashCalculator;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;

public class RandomizedHashCalculator : HashCalculatorBase, IHashCalculator
{
    protected override IHashAlgorithmAdapter Algorithm {
        get => base.Algorithm;
        set => base.Algorithm = value; 
        }
    protected int _seed=0;
    public RandomizedHashCalculator(int seed)
    {
        _seed = seed;
    }
}
