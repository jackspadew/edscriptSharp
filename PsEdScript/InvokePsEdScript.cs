using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using LibEd.EdData;
using System.Text;

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
        var plainText = Encoding.UTF8.GetString(plainBytes);
        var scriptResult = Common.InvokeScriptByString(plainText);
        WriteObject(scriptResult);
    }
}
