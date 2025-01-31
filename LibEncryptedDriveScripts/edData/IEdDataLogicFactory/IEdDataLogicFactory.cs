namespace LibEd.EdData;

using LibEd.Database;

public interface IEdDataLogicFactory
{
    IEdDataCryptor CreateCryptor(IEdDataWorker thisInstance);
    IDatabaseOperator CreateDatabaseOperator(IEdDataWorker thisInstance);
    IMultipleKeyExchanger CreateMultipleKeyExchanger(IEdDataWorker thisInstance);
    IMultipleKeyExchanger CreateKeyBlendedMultipleKeyExchanger(IEdDataWorker thisInstance);
    IEdDataWorker CreateWorker();
    IEdDataHashCalculator CreateHashCalculator(IEdDataWorker thisInstance);
    void SetPassword(string password);
}
