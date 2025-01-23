namespace LibEncryptedDriveScripts;

using LibEncryptedDriveScripts.EdData;

public class IntegrationTests_EdData
{
    private byte[] exampleBytes = new byte[]{0,1,2,3};
    private string examplePassword = "Example#Password";
    private string dbPath = "IntegrationTests_EdData.db";
    private string exampleIndex = "IndexNameOfStashedData";
    private string notExistsIndex = "NonExistentIndexName";
}
