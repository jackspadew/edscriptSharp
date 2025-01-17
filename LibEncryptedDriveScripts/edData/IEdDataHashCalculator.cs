namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataHashCalculator
{
    byte[] ComputeHash(byte[] inputBytes);
    byte[] ComputeHash(Stream inputStream);
}
