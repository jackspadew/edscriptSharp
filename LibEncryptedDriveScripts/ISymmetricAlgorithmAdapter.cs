namespace LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

public interface ISymmetricAlgorithmAdapter
{
    int LegalIVSize { get; }
    int LegalKeySize { get; }
    Stream CreateEncryptStream(Stream inputStream, byte[] key, byte[] iv);
    Stream CreateDecryptStream(Stream inputStream, byte[] key, byte[] iv);
}
