using System.Management.Automation;
using LibEd.EdData;
using System;

namespace PsEdScript;

[Cmdlet(VerbsCommon.New,"PsEdScriptLogicObj")]
[OutputType(typeof(IEdDataLogicFactory))]
public class NewPsEdScriptLogicObj : PSCmdlet
{
    [Parameter(
        Mandatory = false
        )]
    public string Path { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public string Password { get; set; }

    private int _hashStretchingCount = Common.DEFAULT_HASH_STRETCHING_COUNT;
    [Parameter(
        Mandatory = false
        )]
    public int HashStretchingCount {
        get => _hashStretchingCount;
        set {
            int lowerLimit = 1;
            if(value < lowerLimit)
            {
                ThrowExceptionBelowTheLowerLimit(nameof(HashStretchingCount), value, lowerLimit);
            }
            _hashStretchingCount = value;
        }
    }

    private int _multipleEncryptionCount = Common.DEFAULT_MULTIPLE_ENCRYPTION_COUNT;
    [Parameter(
        Mandatory = false
        )]
    public int MultipleEncryptionCount {
        get => _multipleEncryptionCount;
        set {
            int lowerLimit = 1;
            if(value < lowerLimit)
            {
                ThrowExceptionBelowTheLowerLimit(nameof(MultipleEncryptionCount), value, lowerLimit);
            }
            _multipleEncryptionCount = value;
        }
    }

    private int _fakeInsertionCount = Common.DEFAULT_FAKEINSERTION_COUNT;
    [Parameter(
        Mandatory = false
        )]
    public int FakeInsertionCount {
        get => _fakeInsertionCount;
        set {
            int lowerLimit = 0;
            if(value < lowerLimit)
            {
                ThrowExceptionBelowTheLowerLimit(nameof(FakeInsertionCount), value, lowerLimit);
            }
            _fakeInsertionCount = value;
        }
    }

    protected virtual ArgumentOutOfRangeException ThrowExceptionBelowTheLowerLimit(string name, int actual, int limit)
    {
        throw new ArgumentOutOfRangeException(name, $"An attempt was made to assign to \"{name}\" a value below {limit}. (value: {actual})");
    }

    protected override void BeginProcessing()
    {
        IEdDataLogicFactory logicObj = Common.GenerateEdDataLogicObject(Path, Password,
            HashStretchingCount: HashStretchingCount,
            MultipleEncryptionCount: MultipleEncryptionCount,
            FakeInsertionCount: FakeInsertionCount);
        WriteObject(logicObj);
    }
}
