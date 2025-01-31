namespace LibEd.SaltStream;

using System.IO;

public class SaltStream : Stream
{
    private readonly Stream _baseStream;
    private readonly byte[] _salt;
    private int _insertedCount = 0;
    private byte[] _nextInsertBytes = new byte[0];
    private long? _nextInsertPosition;
    private List<long> _insertPositionList = new();
    private bool _insertTailFlag = false;
    private bool _isBeingInserted => _nextInsertBytes.Length != 0;

    public SaltStream(Stream baseStream, byte[] salt, long[] listInsertPos, bool insertTail=false)
    {
        _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        _salt = salt ?? throw new ArgumentNullException(nameof(salt));
        _insertPositionList = listInsertPos.Where(n => n >= 0).ToList();
        _insertTailFlag = insertTail;
        UpdateNextInsertPosition();
    }

    public SaltStream(Stream baseStream, byte[] salt, bool insertTail=false) : this(baseStream, salt, new long[]{}, insertTail) {}

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => _baseStream.Position + _insertedCount;
        set => throw new NotSupportedException();
    }
    public override void Flush()
    {
        throw new NotSupportedException();
    }
    private void UpdateNextInsertPosition()
    {
        if(_isBeingInserted) return;
        if(_insertPositionList.Count != 0)
        {
            _nextInsertPosition = _insertPositionList.Min();
            _insertPositionList.Remove(_nextInsertPosition ?? -1);
        }
    }
    private void UpdateNextInsertBytes()
    {
        _nextInsertBytes = _salt;
        _nextInsertPosition = null;
    }
    public override int Read(byte[] buffer, int offset, int count)
    {
        int readTotal = 0;
        int countRemain = count;
        bool readNothing = false;

        while(true)
        {
            if(countRemain == 0) break;
            if( _nextInsertPosition == _baseStream.Position )
            {
                UpdateNextInsertBytes();
            }
            if(readNothing && _insertTailFlag)
            {
                UpdateNextInsertBytes();
                _insertTailFlag = false;
            }
            if(_isBeingInserted)
            {
                int insertableLength = Math.Min(countRemain, buffer.Length - offset);
                int insertLength = Math.Min(_nextInsertBytes.Length, insertableLength);
                if(insertLength == _nextInsertBytes.Length)
                {
                    Array.Copy(_nextInsertBytes, 0, buffer, offset, insertLength);
                    _nextInsertBytes = new byte[0];
                    UpdateNextInsertPosition();
                }
                else
                {
                    Array.Copy(_nextInsertBytes, 0, buffer, offset, insertLength);
                    _nextInsertBytes = _nextInsertBytes[insertLength..];
                }
                _insertedCount += insertLength;
                offset += insertLength;
                countRemain -= insertLength;
                readTotal += insertLength;
                continue;
            }

            if(readNothing) break;
            int toReadFromSourceCount;
            if(_nextInsertPosition == null){
                toReadFromSourceCount = countRemain;
            }
            else
            {
                toReadFromSourceCount = Math.Min(countRemain, (int)(_nextInsertPosition - _baseStream.Position));
            }
            if(toReadFromSourceCount > 0)
            {
                int readFromSource = _baseStream.Read(buffer, offset, toReadFromSourceCount);
                if(readFromSource == 0) readNothing = true;
                readTotal += readFromSource;
                offset += readFromSource;
                countRemain -= readFromSource;
                continue;
            }
        }

        return readTotal;
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("This stream is read-only.");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _baseStream.Dispose();
        }
        base.Dispose(disposing);
    }
}
