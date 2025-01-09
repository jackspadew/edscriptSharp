namespace LibEncryptedDriveScripts.KeyGenerator;

public interface IListGenerator<T>
{
    List<T> Generate(int length);
}
