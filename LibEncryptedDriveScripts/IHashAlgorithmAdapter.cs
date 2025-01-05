namespace LibEncryptedDriveScripts.HashAlgorithmAdapter;

public interface IHashAlgorithmAdapter
{
    byte[] ComputeHash(byte[] inputBytes);
    byte[] ComputeHash(Stream inputStream);
    byte[] ComputeHash(byte[] inputBytes, byte[] salt);
    byte[] ComputeHash(Stream inputStream, byte[] salt);
}
