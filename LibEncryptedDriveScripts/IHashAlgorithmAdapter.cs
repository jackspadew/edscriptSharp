namespace LibEncryptedDriveScripts.HashAlgorithmAdapter;

public interface IHashAlgorithmAdapter
{
    byte[] ComputeHash(byte[] inputBytes);
    byte[] ComputeHash(Stream inputStream);
    byte[] ComputeHash(byte[] inputBytes, byte[] solt);
    byte[] ComputeHash(Stream inputStream, byte[] solt);
}
