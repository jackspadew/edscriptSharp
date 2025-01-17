namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public interface IEdDataLogicFactory
{
    IEdDataCryptor CreateCryptor();
    IDatabaseOperator CreateDatabaseOperator();
    IMultipleKeyExchanger CreateMultipleKeyExchanger();
    IEdDataWorker CreateWorker();
    IEdDataHashCalculator CreateHashCalculator();
}
