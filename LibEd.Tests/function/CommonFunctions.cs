namespace LibEd.Tests;

using LibEd.EdData;

public static class CommonFunctions
{
    public static void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static IEdDataLogicFactory CreateLightweigthLogicFactory(string dbPath, string password)
    {
        return new BasicEdDataLogicFactory(dbPath, password,
            5, 5, 5, 5, 5,
            5, 5, 5, 5, 5,
            5, 1);
    }
}
