namespace LibEncryptedDriveScripts.EdData;

public interface IEdDataPlanter
{
    void Stash(string index, byte[] data);
}

public interface IEdDataExtractor
{
    byte[] Extract(string index);
}
