namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Converter;
using System.Security.Cryptography;

public abstract class MultipleKeyExchangerBase : IMultipleKeyExchanger
{
    public int KeySeed { get; set; }
    public int IVSeed { get; set; }
    public int AlgorithmSeed { get; set; }
    public int HashSeed { get; set; }
    protected byte[] _key = new byte[32];
    public byte[] Key {
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
    public byte[] IV {
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
    public byte[] Salt {
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
    public byte[] Lye {
        get => _lye;
        set {
            if(value.Length != _lye.Length)
            {
                throw new IndexOutOfRangeException();
            }
            _lye = (byte[])value.Clone();
        }
        }
    public static int BytesLength = (4 * 4) + 32 + 16 + (32 * 2);

    public byte[] GetBytes()
    {
        List<byte[]> list = new();
        list.Add(BitConverter.GetBytes(KeySeed));
        list.Add(BitConverter.GetBytes(IVSeed));
        list.Add(BitConverter.GetBytes(AlgorithmSeed));
        list.Add(BitConverter.GetBytes(HashSeed));
        list.Add(Key);
        list.Add(IV);
        list.Add(Salt);
        list.Add(Lye);
        var converter = new BytesListToCombinedBytesConverter();
        return converter.Convert(list);
    }

    public void SetBytes(byte[] inputBytes)
    {
        if(inputBytes.Length != BytesLength)
        {
            throw new ArgumentOutOfRangeException("The length of the given byte[] does not match the legal length.");
        }
        int currentPos = 0;
        this.KeySeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += 4;
        this.IVSeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += 4;
        this.AlgorithmSeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += 4;
        this.HashSeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += 4;
        Array.Copy(inputBytes, currentPos, this.Key, 0, 32);
        currentPos += 32;
        Array.Copy(inputBytes, currentPos, this.IV, 0, 16);
        currentPos += 16;
        Array.Copy(inputBytes, currentPos, this.Salt, 0, this.Salt.Length);
        currentPos += this.Salt.Length;
        Array.Copy(inputBytes, currentPos, this.Lye, 0, this.Lye.Length);
        currentPos += this.Lye.Length;
    }

    public void CopyTo(IMultipleKeyExchanger targetMultiKey)
    {
        targetMultiKey.KeySeed = this.KeySeed;
        targetMultiKey.IVSeed = this.IVSeed;
        targetMultiKey.AlgorithmSeed = this.AlgorithmSeed;
        targetMultiKey.HashSeed = this.HashSeed;
        targetMultiKey.Key = this.Key;
        targetMultiKey.IV = this.IV;
        targetMultiKey.Salt = this.Salt;
        targetMultiKey.Lye = this.Lye;
    }

    public void Randomize()
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        this.KeySeed = RandomNumberGenerator.GetInt32(int.MaxValue);
        this.IVSeed = RandomNumberGenerator.GetInt32(int.MaxValue);
        this.AlgorithmSeed = RandomNumberGenerator.GetInt32(int.MaxValue);
        this.HashSeed = RandomNumberGenerator.GetInt32(int.MaxValue);
        rng.GetBytes(Key);
        rng.GetBytes(IV);
        rng.GetBytes(Salt);
        rng.GetBytes(Lye);
    }
}
