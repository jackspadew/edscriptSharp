namespace LibEd.KeyGenerator;

public interface IListGenerator<T>
{
    List<T> Generate(int length);
}
