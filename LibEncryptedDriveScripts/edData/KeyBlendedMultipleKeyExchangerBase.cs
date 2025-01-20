namespace LibEncryptedDriveScripts.EdData;

public class KeyBlendedMultipleKeyExchangerBase : MultipleKeyExchangerBase, IMultipleKeyExchanger
{
    public override int KeySeed { get; set; }
    public override int IVSeed { get; set; }
    public override int AlgorithmSeed { get; set; }
    public override int HashSeed { get; set; }
    public override byte[] Key { get; set; } = new byte[32];
    public override byte[] IV { get; set; } = new byte[16];
    public override byte[] Salt { get; set; } = new byte[32];
    public override byte[] Lye { get; set; } = new byte[32];
    protected override int BytesLength => (4 * 4) + 32 + 16 + (32 * 2);
}
