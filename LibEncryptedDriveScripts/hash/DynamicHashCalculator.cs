namespace LibEncryptedDriveScripts.HashCalculator;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;

public class DynamicHashCalculator : HashCalculatorBase, IHashCalculator
{
    public DynamicHashCalculator(IHashAlgorithmAdapter hashAlgorithm) : base(hashAlgorithm)
    {}
}
