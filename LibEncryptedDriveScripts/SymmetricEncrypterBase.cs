namespace LibEncryptedDriveScripts.SymmetricEncrypter;

using LibEncryptedDriveScripts.SymmetricAlgorithmAdapter;

public abstract class SymmetricEncrypterBase : ISymmetricEncrypter
{
    protected List<ISymmetricAlgorithmAdapter> _algorithm = new();

    protected virtual byte[] KeyConverter(byte[] inputBytes)
    {
        return inputBytes;
    }
    protected virtual byte[] IVConverter(byte[] inputBytes)
    {
        return inputBytes;
    }
    public virtual byte[] Encrypt(byte[] inputBytes, byte[] key, byte[] iv)
    {
        byte[] tmpBytes = inputBytes;
        byte[] tmpKey = key;
        byte[] tmpIV = iv;
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm)
        {
            tmpKey = KeyConverter(tmpKey);
            tmpIV = IVConverter(tmpIV);
            tmpBytes = algo.EncryptBytes(tmpBytes, tmpKey, tmpIV);
        }
        return tmpBytes;
    }
    public virtual byte[] Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        byte[] tmpBytes = encryptedBytes;
        byte[] tmpKey = key;
        byte[] tmpIV = iv;
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm.AsEnumerable().Reverse())
        {
            tmpKey = KeyConverter(tmpKey);
            tmpIV = IVConverter(tmpIV);
            tmpBytes = algo.DecryptBytes(tmpBytes, tmpKey, tmpIV);
        }
        return tmpBytes;
    }

    protected virtual void DisposeCreatedStreams(List<Stream> streamList)
    {
        foreach(Stream stream in streamList)
        {
            stream.Dispose();
        }
    }
    public virtual void Encrypt(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
    {
        List<Stream> createdStreamList = new();
        Stream nextOutputStream = outputStream;
        byte[] tmpKey = key;
        byte[] tmpIV = iv;
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm.AsEnumerable().Reverse())
        {
            tmpKey = KeyConverter(tmpKey);
            tmpIV = IVConverter(tmpIV);
            nextOutputStream = algo.CreateWritableEncryptStream(nextOutputStream, tmpKey, tmpIV);
            createdStreamList.Insert(0, nextOutputStream);
        }
        Stream FirstEncryptStream = nextOutputStream;
        inputStream.CopyTo(FirstEncryptStream);
        FirstEncryptStream.Dispose();
        DisposeCreatedStreams(createdStreamList);
    }
    public virtual void Decrypt(Stream encryptedStream, Stream outputStream, byte[] key, byte[] iv)
    {
        List<Stream> createdStreamList = new();
        Stream nextOutputStream = outputStream;
        byte[] tmpKey = key;
        byte[] tmpIV = iv;
        foreach(ISymmetricAlgorithmAdapter algo in _algorithm)
        {
            tmpKey = KeyConverter(tmpKey);
            tmpIV = IVConverter(tmpIV);
            nextOutputStream = algo.CreateWritableDecryptStream(nextOutputStream, tmpKey, tmpIV);
            createdStreamList.Insert(0, nextOutputStream);
        }
        Stream FirstDecryptStream = nextOutputStream;
        encryptedStream.CopyTo(FirstDecryptStream);
        FirstDecryptStream.Dispose();
        DisposeCreatedStreams(createdStreamList);
    }
}
