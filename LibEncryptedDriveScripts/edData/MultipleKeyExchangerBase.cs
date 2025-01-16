namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Converter;

public abstract class MultipleKeyExchangerBase : IMultipleKeyExchanger
{
    public int KeySeed { get; set; }
    public int IVSeed { get; set; }
    public int AlgorithmSeed { get; set; }
    public int HashSeed { get; set; }
    public byte[] Key { get; set; } = new byte[32];
    public byte[] IV { get; set; } = new byte[16];
    public static int BytesLength = (4 * 4) + 32 + 16;

    public byte[] GetBytes()
    {
        List<byte[]> list = new();
        list.Add(BitConverter.GetBytes(KeySeed));
        list.Add(BitConverter.GetBytes(IVSeed));
        list.Add(BitConverter.GetBytes(AlgorithmSeed));
        list.Add(BitConverter.GetBytes(HashSeed));
        list.Add(Key);
        list.Add(IV);
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
    }

    public void CopyTo(IMultipleKeyExchanger targetMultiKey)
    {
        targetMultiKey.KeySeed = this.KeySeed;
        targetMultiKey.IVSeed = this.IVSeed;
        targetMultiKey.AlgorithmSeed = this.AlgorithmSeed;
        targetMultiKey.HashSeed = this.HashSeed;
        targetMultiKey.Key = this.Key;
        targetMultiKey.IV = this.IV;
    }
}
