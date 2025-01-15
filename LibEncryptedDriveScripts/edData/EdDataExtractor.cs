namespace LibEncryptedDriveScripts.EdData;

using LibEncryptedDriveScripts.Database;

public class EdDataExtractor : EdDataWorkerBase, IEdDataExtractor, IEdDataPlanter
{
    protected override IDatabaseOperator DbOperator => throw new NotImplementedException();

    protected override IEdDataCryptor EdCryptor => throw new NotImplementedException();

    protected override IMultipleKeyExchanger MultipleKey => throw new NotImplementedException();

    protected override byte[] GenerateIndexBytes(string name)
    {
        throw new NotImplementedException();
    }
}

public class StashedMultipleKeyExchanger : MultipleKeyExchangerBase, IMultipleKeyExchanger
{}
