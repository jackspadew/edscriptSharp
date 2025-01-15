namespace LibEncryptedDriveScripts.HashCalculator;

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
        byte[] result = new byte[hashBytes.Length + LyeBytes.Length];
        Array.Copy(hashBytes, 0, result, 0, hashBytes.Length);
        Array.Copy(LyeBytes, 0, result, hashBytes.Length, LyeBytes.Length);
        return result;
    }
}
