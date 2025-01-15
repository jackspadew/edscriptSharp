namespace LibEncryptedDriveScripts.EdData;

public abstract class MultipleKeyExchangerBase : IMultipleKeyExchanger
{
    public int KeySeed { get; set; }
    public int IVSeed { get; set; }
    public int AlgorithmSeed { get; set; }
    public byte[] Key { get; set; } = new byte[32];
    public byte[] IV { get; set; } = new byte[16];

    public byte[] GetBytes()
    {
        throw new NotImplementedException();
    }

    public void SetBytes(byte[] inputBytes)
    {
        throw new NotImplementedException();
    }
}
