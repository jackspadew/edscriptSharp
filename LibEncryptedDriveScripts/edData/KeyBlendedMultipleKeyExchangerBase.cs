namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Converter;

public class KeyBlendedMultipleKeyExchangerBase : MultipleKeyExchangerBase, IMultipleKeyExchanger
{
    private int _keySeed = 0;
    public override int KeySeed {
        get => KeyBlendedInt(_keySeed);
        set => _keySeed = value;
        }
    private int _ivSeed = 0;
    public override int IVSeed {
        get => KeyBlendedInt(_ivSeed);
        set => _ivSeed = value;
        }
    private int _algorithmSeed = 0;
    public override int AlgorithmSeed {
        get => KeyBlendedInt(_algorithmSeed);
        set => _algorithmSeed = value;
        }
    private int _hashSeed = 0;
    public override int HashSeed {
        get => KeyBlendedInt(_hashSeed);
        set => _hashSeed = value;
        }
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
        get => KeyBlendedBytes(_iv);
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
        get => KeyBlendedBytes(_salt);
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
        get => KeyBlendedBytes(_lye);
        set {
            if(value.Length != _lye.Length)
            {
                throw new IndexOutOfRangeException();
            }
            _lye = (byte[])value.Clone();
        }
        }
    protected override int BytesLength => (4 * 4) + 32 + 16 + (32 * 2);

    protected byte[] BlendBytes(byte[] originBytes, byte[] additiveBytes)
    {
        var converter = new BytesXorBlendConverter(additiveBytes);
        return converter.Convert(originBytes);
    }
    protected int KeyBlendedInt(int originValue)
    {
        byte[] originBytes = BitConverter.GetBytes(originValue);
        byte[] blendedBytes = BlendBytes(originBytes, Key);
        return BitConverter.ToInt32(blendedBytes);
    }
    protected byte[] KeyBlendedBytes(byte[] originBytes)
    {
        return BlendBytes(originBytes, Key);
    }
    public override byte[] GetBytes()
    {
        List<byte[]> list =
        [
            BitConverter.GetBytes(_keySeed),
            BitConverter.GetBytes(_ivSeed),
            BitConverter.GetBytes(_algorithmSeed),
            BitConverter.GetBytes(_hashSeed),
            Key,
            _iv,
            _salt,
            _lye,
        ];
        var converter = new BytesListToCombinedBytesConverter();
        return converter.Convert(list);
    }
}
