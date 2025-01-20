namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public interface IEdDataLogicFactory
{
    IEdDataCryptor CreateCryptor(IEdDataWorker worker);
    IDatabaseOperator CreateDatabaseOperator(IEdDataWorker worker);
    IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker worker);
    IEdDataWorker CreateWorker(IEdDataWorker worker);
    IEdDataHashCalculator CreateHashCalculator(IEdDataWorker worker);
}
