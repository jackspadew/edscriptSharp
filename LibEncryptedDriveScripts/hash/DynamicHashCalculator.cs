namespace LibEncryptedDriveScripts.HashCalculator;

using LibEncryptedDriveScripts.HashAlgorithmAdapter;

public class DynamicHashCalculator : HashCalculatorBase, IHashCalculator
{
    private IHashAlgorithmAdapter _algorithm;
    protected override IHashAlgorithmAdapter Algorithm => _algorithm;
    public DynamicHashCalculator(IHashAlgorithmAdapter hashAlgorithm)
    {
        _algorithm = hashAlgorithm;
    }
}
