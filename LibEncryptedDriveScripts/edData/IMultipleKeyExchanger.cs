namespace LibEncryptedDriveScripts.EdData;

public interface IMultipleKeyExchanger
{
    void SetBytes(byte[] inputBytes);
    byte[] GetBytes();
    void CopyTo(IMultipleKeyExchanger targetMultiKey);
    int KeySeed {get; set;}
    int IVSeed {get; set;}
    int AlgorithmSeed {get; set;}
    int HashSeed {get; set;}
    byte[] Key {get; set;}
    byte[] IV {get; set;}
    void Randomize();
}
