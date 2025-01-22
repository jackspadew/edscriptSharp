namespace LibEncryptedDriveScripts.EdData;

public class InitialMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase, IMultipleKeyExchanger
{
    public InitialMultipleKeyExchanger()
    {
        Random random = new Random(20626197);
        KeySeed = 90849388;
        IVSeed = 88871264;
        AlgorithmSeed = 93476436;
        byte[] key = new byte[32];
        random.NextBytes(key);
        byte[] iv = new byte[16];
        random.NextBytes(iv);
        Key = key;
        IV = iv;
    }
}

public class BasicExemplaryMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase, IMultipleKeyExchanger
{

}

public class BasicKeyBlendedMultipleKeyExchanger : KeyBlendedMultipleKeyExchangerBase, IMultipleKeyExchanger
{

}
