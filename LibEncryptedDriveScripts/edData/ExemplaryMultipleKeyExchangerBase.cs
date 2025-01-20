namespace LibEncryptedDriveScripts.EdData;

public class ExemplaryMultipleKeyExchangerBase : MultipleKeyExchangerBase, IMultipleKeyExchanger
{
    public override int KeySeed { get; set; }
    public override int IVSeed { get; set; }
    public override int AlgorithmSeed { get; set; }
    public override int HashSeed { get; set; }
    protected byte[] _key = new byte[32];
    public override byte[] Key {
        get => _key;
        set {
            if(value.Length != _key.Length)
            {
                throw new IndexOutOfRangeException();
            }
            _key = (byte[])value.Clone();
        }
        }
    protected byte[] _iv = new byte[16];
    public override byte[] IV {
        get => _iv;
        set {
            if(value.Length != _iv.Length)
            {
                throw new IndexOutOfRangeException();
            }
            _iv = (byte[])value.Clone();
        }
        }
    protected byte[] _salt = new byte[32];
    public override byte[] Salt {
        get => _salt;
        set {
            if(value.Length != _salt.Length)
            {
                throw new IndexOutOfRangeException();
            }
            _salt = (byte[])value.Clone();
        }
        }
    protected byte[] _lye = new byte[32];
    public override byte[] Lye {
        get => _lye;
        set {
            if(value.Length != _lye.Length)
            {
                throw new IndexOutOfRangeException();
            }
            _lye = (byte[])value.Clone();
        }
        }
    protected override int BytesLength => (4 * 4) + 32 + 16 + (32 * 2);
}
