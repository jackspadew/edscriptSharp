using System.Management.Automation;
using LibEd.EdData;

namespace PsEdScript;

[Cmdlet(VerbsCommon.New,"PsEdScriptLogicObj")]
[OutputType(typeof(IEdDataLogicFactory))]
public class NewPsEdScriptLogicObj : PSCmdlet
{
    [Parameter(
        Mandatory = true
        )]
    public string Path { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public string Password { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public int HashStretchingCount { get; set; } = Common.DEFAULT_HASH_STRETCHING_COUNT;

    [Parameter(
        Mandatory = false
        )]
    public int MultipleEncryptionCount { get; set; } = Common.DEFAULT_MULTIPLE_ENCRYPTION_COUNT;

    [Parameter(
        Mandatory = false
        )]
    public int FakeInsertionCount { get; set; } = Common.DEFAULT_FAKEINSERTION_COUNT;

    protected override void BeginProcessing()
    {
        IEdDataLogicFactory logicObj = Common.GenerateEdDataLogicObject(Path, Password,
            HashStretchingCount: HashStretchingCount,
            MultipleEncryptionCount: MultipleEncryptionCount,
            FakeInsertionCount: FakeInsertionCount);
        WriteObject(logicObj);
    }
}
