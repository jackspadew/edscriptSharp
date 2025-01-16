namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public class EdDataKeyMakingWorker : EdDataWorkerBase, IEdDataWorker
{
    protected override IDatabaseOperator DbOperator => throw new NotImplementedException();

    protected override IEdDataCryptor EdCryptor => throw new NotImplementedException();

    protected override IMultipleKeyExchanger MultipleKey => throw new NotImplementedException();

    protected override byte[] GenerateIndexBytes(string name)
    {
        throw new NotImplementedException();
    }
}

public class KeyMakerMultipleKeyExchanger : MultipleKeyExchangerBase, IMultipleKeyExchanger
{}
