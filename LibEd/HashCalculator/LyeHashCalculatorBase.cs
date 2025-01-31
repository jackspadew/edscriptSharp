namespace LibEd.HashCalculator;

public abstract class LyeHashCalculatorBase : HashCalculatorBase, IHashCalculator
{
    protected virtual byte[] LyeBytes {get; set;} = new byte[0];
    protected override byte[] Stretching(byte[] hashBytes, int stretchCount)
    {
        byte[] tmpHash = hashBytes;
        for(int i=1; i < stretchCount; i++)
        {
            tmpHash = LyeTreatment(tmpHash);
            tmpHash = ComputeHashByGivenAlgorithm(Algorithm, tmpHash);
        }
        return tmpHash;
    }
    protected virtual byte[] LyeTreatment(byte[] hashBytes)
    {
        byte[] lyeBytes = this.LyeBytes;
        byte[] result = new byte[hashBytes.Length + lyeBytes.Length];
        Array.Copy(hashBytes, 0, result, 0, hashBytes.Length);
        Array.Copy(lyeBytes, 0, result, hashBytes.Length, lyeBytes.Length);
        return result;
    }
}
