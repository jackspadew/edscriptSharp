namespace LibEncryptedDriveScripts.Tests;

public static class CommonFunctions
{
    public static void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
