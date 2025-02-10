namespace LibEd.PerformanceTests;

using System.Reflection;
using LibEd.HashAlgorithmAdapter;

#pragma warning disable CS8602

public class HashAlgorithm_PerfTests
{
    private static byte[] exampleBytes = new byte[]{0,1,2,3};
    public static IEnumerable<object[]> HashAlgorithObjects()
    {
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.SHA3(), "BouncyCastle.SHA3" };
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.BLAKE3(), "BouncyCastle.BLAKE3" };
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.BLAKE2b(), "BouncyCastle.BLAKE2b" };
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.Skein(), "BouncyCastle.Skein" };
        yield return new object[] { new HashAlgorithmAdapter.BouncyCastle.Whirlpool(), "BouncyCastle.Whirlpool" };
    }

    [Theory]
    [MemberData(nameof(HashAlgorithObjects))]
    public void ComputeHash_WillReturnThatHaveEnoughLength(IHashAlgorithmAdapter hashAlgo, string className)
    {
        byte[] bytes = exampleBytes;
        PerformanceTestCommon.CompletesIn($"{MethodBase.GetCurrentMethod().Name} : {className}", 50000, () => {
            for(int i=0; i<10000; i++)
            {
                bytes = hashAlgo.ComputeHash(bytes);
            }
        });
    }
}
