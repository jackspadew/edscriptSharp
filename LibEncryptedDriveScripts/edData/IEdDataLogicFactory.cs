namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.SymmetricEncrypter;
using LibEncryptedDriveScripts.Database;

public interface IEdDataLogicFactory
{
    IEdDataCryptor CreateCryptor();
    IDatabaseOperator CreateDatabaseOperator();
    IMultipleKeyExchanger CreateMultipleKeyExchangerGeneratedRandomly();
    IMultipleKeyExchanger CreateMultipleKeyExchangerBlendedWithSecretKey();
    IEdDataWorker CreateWorker();
}
