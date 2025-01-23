namespace LibEncryptedDriveScripts.HashAlgorithmAdapter;

public interface IHashAlgorithmAdapter
{
    byte[] ComputeHash(byte[] inputBytes);
    byte[] ComputeHash(Stream inputStream);
}
