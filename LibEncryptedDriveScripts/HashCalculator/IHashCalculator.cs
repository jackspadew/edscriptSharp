namespace LibEncryptedDriveScripts.HashCalculator;

public interface IHashCalculator
{
    byte[] ComputeHash(byte[] inputBytes, byte[] salt, int stretchCount=1);
    byte[] ComputeHash(byte[] inputBytes, int stretchCount=1);
    byte[] ComputeHash(Stream inputStream, byte[] salt, int stretchCount=1);
    byte[] ComputeHash(Stream inputStream, int stretchCount=1);
}
