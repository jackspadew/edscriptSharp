namespace LibEd.HashCalculator;

using LibEd.HashAlgorithmAdapter;

public class DynamicHashCalculator : HashCalculatorBase, IHashCalculator
{
    private IHashAlgorithmAdapter _algorithm;
    protected override IHashAlgorithmAdapter Algorithm => _algorithm;
    public DynamicHashCalculator(IHashAlgorithmAdapter hashAlgorithm)
    {
        _algorithm = hashAlgorithm;
    }
}
