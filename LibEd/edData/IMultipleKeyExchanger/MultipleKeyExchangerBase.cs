namespace LibEd.EdData;

using LibEd.Converter;
using Pcg;
using System.Security.Cryptography;

public abstract class MultipleKeyExchangerBase : IMultipleKeyExchanger
{
    protected const int KEY_BYTES_LENGTH = 32;
    protected const int IV_BYTES_LENGTH = 16;
    protected const int SALT_BYTES_LENGTH = 32;
    protected const int INTEGER_BYTES_LENGTH = 4;
    public abstract int KeySeed { get; set; }
    public abstract int IVSeed { get; set; }
    public abstract int AlgorithmSeed { get; set; }
    public abstract int HashSeed { get; set; }
    public abstract byte[] Key { get; set; }
    public abstract byte[] IV { get; set; }
    public abstract byte[] Salt { get; set; }
    public abstract byte[] Lye { get; set; }
    protected abstract int BytesLength {get;}

    public virtual byte[] GetBytes()
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

    public virtual void SetBytes(byte[] inputBytes)
    {
        if(inputBytes.Length != BytesLength)
        {
            throw new ArgumentOutOfRangeException("The length of the given byte[] does not match the legal length.");
        }
        int currentPos = 0;
        this.KeySeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += INTEGER_BYTES_LENGTH;
        this.IVSeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += INTEGER_BYTES_LENGTH;
        this.AlgorithmSeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += INTEGER_BYTES_LENGTH;
        this.HashSeed = BitConverter.ToInt32(inputBytes, currentPos);
        currentPos += INTEGER_BYTES_LENGTH;
        Array.Copy(inputBytes, currentPos, this.Key, 0, this.Key.Length);
        currentPos += this.Key.Length;
        Array.Copy(inputBytes, currentPos, this.IV, 0, this.IV.Length);
        currentPos += this.IV.Length;
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

    public void Randomize(int seed)
    {
        var random = new PcgRandom(seed);
        this.KeySeed = random.Next();
        this.IVSeed = random.Next();
        this.AlgorithmSeed = random.Next();
        this.HashSeed = random.Next();
        byte[] key = new byte[Key.Length];
        random.NextBytes(key);
        byte[] iv = new byte[IV.Length];
        random.NextBytes(iv);
        byte[] salt = new byte[Salt.Length];
        random.NextBytes(salt);
        byte[] lye = new byte[Lye.Length];
        random.NextBytes(lye);
        Key = key;
        IV = iv;
        Salt = salt;
        Lye = lye;
    }

    public override string ToString()
    {
        return $"KeySeed={KeySeed} IVSeed={IVSeed} AlgorithmSeed={AlgorithmSeed} HashSeed={HashSeed} Key=[{BitConverter.ToString(Key)} IV=[{BitConverter.ToString(IV)}] Salt=[{BitConverter.ToString(Salt)}] Lye=[{BitConverter.ToString(Lye)}]";
    }
}
