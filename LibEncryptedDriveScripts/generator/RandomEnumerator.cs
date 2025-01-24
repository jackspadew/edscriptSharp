namespace LibEncryptedDriveScripts.KeyGenerator;

using System;
using System.Collections;
using System.Collections.Generic;

public class RandomEnumerator<T> : IEnumerator<T>
{
    private readonly IList<T> _list;
    private readonly Random _random;
    public RandomEnumerator(IList<T> list, int seed)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
        _random = new Random(seed);
        MoveNext();
    }
    public RandomEnumerator(IList<T> list) : this(list, 0) {}
    private T? _current;
    public T Current {
        get {
            if(_current == null) throw new InvalidOperationException();
            return _current;
        }
        private set => _current = value;
        }
#pragma warning disable CS8603 // Possible null reference return.
    object IEnumerator.Current => Current;
#pragma warning restore CS8603 // Possible null reference return.

    public bool MoveNext()
    {
        if (_list.Count == 0)
        {
            return false;
        }
        int randomIndex = _random.Next(_list.Count);
        Current = _list[randomIndex];
        return true;
    }
    public void Reset()
    {
        throw new NotSupportedException("Reset is not supported.");
    }
    public void Dispose()
    {}
}
