namespace LibEncryptedDriveScripts.EdData;

using System.IO;
using LibEncryptedDriveScripts.SymmetricEncrypter;

public class EdDataCryptor : IEdDataCryptor
{
    protected int MultipleEncryptCount;
    public EdDataCryptor(int multipleEncryptCount)
    {
        this.MultipleEncryptCount = multipleEncryptCount;
    }
    public EdDataCryptor() : this(1000) {}

    public byte[] EncryptBytes(byte[] dataBytes, IMultipleKeyExchanger multiKey)
    {
        ISymmetricEncrypter encrypter = CreateEncrypter(multiKey);
        return encrypter.Encrypt(dataBytes, multiKey.Key, multiKey.IV);
    }
    public byte[] DecryptBytes(byte[] dataBytes, IMultipleKeyExchanger multiKey)
    {
        ISymmetricEncrypter encrypter = CreateEncrypter(multiKey);
        return encrypter.Decrypt(dataBytes, multiKey.Key, multiKey.IV);
    }
    public void EncryptStream(Stream dataStream, Stream outputStream, IMultipleKeyExchanger multiKey)
    {
        ISymmetricEncrypter encrypter = CreateEncrypter(multiKey);
        encrypter.Encrypt(dataStream, outputStream, multiKey.Key, multiKey.IV);
    }
    public void DecryptStream(Stream dataStream, Stream outputStream, IMultipleKeyExchanger multiKey)
    {
        ISymmetricEncrypter encrypter = CreateEncrypter(multiKey);
        encrypter.Decrypt(dataStream, outputStream, multiKey.Key, multiKey.IV);
    }

    protected virtual ISymmetricEncrypter CreateEncrypter(IMultipleKeyExchanger multiKey)
    {
        var encrypter = new RandomizedMultipleEncrypter(multiKey.KeySeed, multiKey.IVSeed, multiKey.AlgorithmSeed, MultipleEncryptCount);
        return encrypter;
    }
}
