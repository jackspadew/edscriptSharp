namespace LibEncryptedDriveScripts.Converter;

public interface IConverter<T,U>
{
    U Convert(T input);
}
