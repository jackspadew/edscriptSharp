namespace LibEd.EdData;

using LibEd.Converter;

public class InitialMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase, IMultipleKeyExchanger
{
    public InitialMultipleKeyExchanger()
    {
        Randomize(20626197);
    }
}

public class BasicExemplaryMultipleKeyExchanger : ExemplaryMultipleKeyExchangerBase, IMultipleKeyExchanger
{

}

public class BasicKeyBlendedMultipleKeyExchanger : KeyBlendedMultipleKeyExchangerBase, IMultipleKeyExchanger
{
    protected override IConverter<byte[], byte[]> CreateKeyBlendConverter(byte[] additiveBytes)
    {
        return new BytesXorBlendConverter(additiveBytes);
    }
}
