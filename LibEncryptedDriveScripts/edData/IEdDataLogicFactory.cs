namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public interface IEdDataLogicFactory
{
    IEdDataCryptor CreateCryptor(IEdDataWorker thisInstance);
    IDatabaseOperator CreateDatabaseOperator(IEdDataWorker thisInstance);
    IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker thisInstance);
    IEdDataWorker CreateWorker();
    IEdDataHashCalculator CreateHashCalculator(IEdDataWorker thisInstance);
}
