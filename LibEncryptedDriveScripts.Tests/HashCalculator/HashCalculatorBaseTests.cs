namespace LibEd.Tests;

using Xunit;
using Moq;
using LibEd.HashCalculator;
using LibEd.HashAlgorithmAdapter;

#pragma warning disable xUnit1026 // Unused arguments

public class HashCalculatorBase_Tests
{
    public class HashCalculator_WithGivenAlgorithm : HashCalculatorBase, IHashCalculator
    {
        private IHashAlgorithmAdapter _algorithm;
        protected override IHashAlgorithmAdapter Algorithm => _algorithm;
        public HashCalculator_WithGivenAlgorithm(IHashAlgorithmAdapter hashAlgorithm)
        {
            _algorithm = hashAlgorithm;
        }
    }
    public static byte[] exampleBytes = {0,1,0,0};
    public static MemoryStream exampleStream => new(exampleBytes);
    public static byte[] anotherBytes = {0,0,0,0};
    public static MemoryStream anotherStream => new(anotherBytes);
    public static byte[] exampleSalt = {0,0};

    [Fact]
    public void ComputeHashWithStretching_CallCountIsSame()
    {
        int stretchingCount = 10;
        var mockedAlgorithm = new Mock<IHashAlgorithmAdapter>();
        mockedAlgorithm.Setup(a => a.ComputeHash(It.IsAny<byte[]>()));
        var hashCalculator = new HashCalculator_WithGivenAlgorithm(mockedAlgorithm.Object);
        var hash = hashCalculator.ComputeHash(exampleBytes, stretchingCount);
        mockedAlgorithm.Verify(a => a.ComputeHash(It.IsAny<byte[]>()), Times.Exactly(stretchingCount));
    }

    [Fact]
    public void ComputeHashWithStretchingWithSalt_CallCountIsSame()
    {
        int stretchingCount = 10;
        var mockedAlgorithm = new Mock<IHashAlgorithmAdapter>();
        mockedAlgorithm.Setup(a => a.ComputeHash(It.IsAny<byte[]>()));
        var hashCalculator = new HashCalculator_WithGivenAlgorithm(mockedAlgorithm.Object);
        var hash = hashCalculator.ComputeHash(exampleBytes, exampleSalt , stretchingCount);
        mockedAlgorithm.Verify(a => a.ComputeHash(It.IsAny<byte[]>()), Times.Exactly(stretchingCount));
    }

    [Fact]
    public void ComputeHashCalledByHashLengthValue_CalledByHashLengthBytesCountIsCorrect()
    {
        int stretchingCount = 10;
        int hashLength = 512;
        byte[] mockResultHash = new byte[hashLength];
        var mockedAlgorithm = new Mock<IHashAlgorithmAdapter>();
        mockedAlgorithm.Setup(a => a.ComputeHash(It.IsAny<byte[]>())).Returns(mockResultHash);
        var hashCalculator = new HashCalculator_WithGivenAlgorithm(mockedAlgorithm.Object);
        var hash = hashCalculator.ComputeHash(exampleBytes, exampleSalt , stretchingCount);
        mockedAlgorithm.Verify(a => a.ComputeHash(It.Is<byte[]>(a => a.Length == hashLength)), Times.Exactly(stretchingCount-1));
    }
}
