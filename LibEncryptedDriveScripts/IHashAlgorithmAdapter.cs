namespace LibEncryptedDriveScripts;

public interface IHashAlgorithmAdapter
{
    byte[] ComputeHash(byte[] inputBytes, byte[] solt);
}
