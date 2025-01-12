namespace LibEncryptedDriveScripts.EdData;

public interface IMultipleKeyExchanger
{
    void SetBytes(byte[] inputBytes);
    byte[] GetBytes();
    int KeySeed {get; set;}
    int IVSeed {get; set;}
    int AlgorithmSeed {get; set;}
    byte[] Key {get; set;}
    byte[] IV {get; set;}
}
