namespace LibEd.Converter;

public interface IConverter<T,U>
{
    U Convert(T input);
}
