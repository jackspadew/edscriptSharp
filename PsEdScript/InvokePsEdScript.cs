using System.Management.Automation;
using LibEd.EdData;

namespace PsEdScript;

[Cmdlet(VerbsLifecycle.Invoke,"PsEdScript")]
public class InvokePsEdScript : PSCmdlet
{
    [Parameter(
        Mandatory = true
        )]
    public string IndexName { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public string Path { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public IEdDataLogicFactory EdDataLogicObject { get; set; }

    protected IEdDataWorker Worker { get; set; }

    protected override void BeginProcessing()
    {
        Worker = Common.GetEdDataWorker(SessionState, EdDataLogicObject, Path);
        var plainBytes = Worker.Extract(IndexName);
        var scriptResult = Common.InvokeScriptByByteArray(plainBytes);
        WriteObject(scriptResult);
    }
}
