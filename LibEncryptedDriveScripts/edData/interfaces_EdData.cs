namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataPlanter
{
    void Insert(string index, byte[] data, IMultipleKeyExchanger multiKey, byte[] key);
}

public interface IEdDataExtractor
{
    byte[] Extract(string index, IMultipleKeyExchanger multiKey, byte[] key);
}
