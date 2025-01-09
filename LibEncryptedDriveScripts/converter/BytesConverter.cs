namespace LibEncryptedDriveScripts.Converter;

public class JustReturnConverter<T> : IConverter<T, T>
{
    public T Convert(T input)
    {
        return input;
    }
}
