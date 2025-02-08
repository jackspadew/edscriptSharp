namespace LibEd.EdData;

using System.Security.Cryptography;
using LibEd.Converter;

public abstract class KeyBlendedMultipleKeyExchangerBase : MultipleKeyExchangerBase, IMultipleKeyExchanger
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
    protected byte[] _key = new byte[KEY_BYTES_LENGTH];
    public override byte[] Key { get => _key; set => _key = value; }
    byte[] IMultipleKeyExchanger.Key {
        get => _key;
        set {
            if(value.Length != _key.Length)
            {
                throw ThrowWrongLengthBytesError(nameof(Key), _key, value);
            }
            _key = (byte[])value.Clone();
        }
        }
    protected byte[] _iv = new byte[IV_BYTES_LENGTH];
    public override byte[] IV { get => _iv; set => _iv = value; }
    byte[] IMultipleKeyExchanger.IV {
        get => KeyBlendedBytes(_iv);
        set {
            if(value.Length != _iv.Length)
            {
                throw ThrowWrongLengthBytesError(nameof(IV), _iv, value);
            }
            _iv = (byte[])value.Clone();
        }
        }
    protected byte[] _salt = new byte[SALT_BYTES_LENGTH];
    public override byte[] Salt { get => _salt; set => _salt = value; }
    byte[] IMultipleKeyExchanger.Salt {
        get => KeyBlendedBytes(_salt);
        set {
            if(value.Length != _salt.Length)
            {
                throw ThrowWrongLengthBytesError(nameof(Salt), _salt, value);
            }
            _salt = (byte[])value.Clone();
        }
        }
    protected byte[] _lye = new byte[SALT_BYTES_LENGTH];
    public override byte[] Lye { get => _lye; set => _lye = value; }
    byte[] IMultipleKeyExchanger.Lye {
        get => KeyBlendedBytes(_lye);
        set {
            if(value.Length != _lye.Length)
            {
                throw ThrowWrongLengthBytesError(nameof(Lye), _lye, value);
            }
            _lye = (byte[])value.Clone();
        }
        }
    protected override int BytesLength => (INTEGER_BYTES_LENGTH * 4) + KEY_BYTES_LENGTH + IV_BYTES_LENGTH + (SALT_BYTES_LENGTH * 2);

    protected abstract IConverter<byte[], byte[]> CreateKeyBlendConverter(byte[] additiveBytes);
    protected byte[] BlendBytes(byte[] originBytes, byte[] additiveBytes)
    {
        IConverter<byte[], byte[]> converter = CreateKeyBlendConverter(additiveBytes);
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
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] fakeKey = new byte[KEY_BYTES_LENGTH];
        rng.GetBytes(fakeKey);
        List<byte[]> list =
        [
            BitConverter.GetBytes(_keySeed),
            BitConverter.GetBytes(_ivSeed),
            BitConverter.GetBytes(_algorithmSeed),
            BitConverter.GetBytes(_hashSeed),
            fakeKey,
            _iv,
            _salt,
            _lye,
        ];
        var converter = new BytesListToCombinedBytesConverter();
        return converter.Convert(list);
    }
    private Exception ThrowWrongLengthBytesError(string valueName, byte[] currentValue, byte[] inputedValue)
    {
        return new IndexOutOfRangeException($"Index was outside the bounds of the array. The length of {valueName} is {currentValue.Length}. But inputted bytes length is {inputedValue.Length}.");
    }
}
