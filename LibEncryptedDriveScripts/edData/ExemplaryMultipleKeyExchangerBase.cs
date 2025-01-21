namespace LibEncryptedDriveScripts.EdData;

public class ExemplaryMultipleKeyExchangerBase : MultipleKeyExchangerBase, IMultipleKeyExchanger
{
    public override int KeySeed { get; set; }
    public override int IVSeed { get; set; }
    public override int AlgorithmSeed { get; set; }
    public override int HashSeed { get; set; }
    protected byte[] _key = new byte[KEY_BYTES_LENGTH];
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
    protected byte[] _iv = new byte[IV_BYTES_LENGTH];
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
    protected byte[] _salt = new byte[SALT_BYTES_LENGTH];
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
    protected byte[] _lye = new byte[SALT_BYTES_LENGTH];
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
    protected override int BytesLength => (INTEGER_BYTES_LENGTH * 4) + KEY_BYTES_LENGTH + IV_BYTES_LENGTH + (SALT_BYTES_LENGTH * 2);
}
