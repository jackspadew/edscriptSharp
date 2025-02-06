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

    protected IEdDataLogicFactory LogicObj { get; set; }
    protected object previousScriptScopeLogicObject { get; set; }

    protected override void BeginProcessing()
    {
        previousScriptScopeLogicObject = SessionState.PSVariable.Get(Common.ScriptScopeLogicObjectName);
        LogicObj = Common.DetermineEdDataLogic(SessionState, EdDataLogicObject, Path);
        var plainBytes = LogicObj.CreateWorker().Extract(IndexName);
        var scriptResult = Common.InvokeScriptByByteArray(plainBytes);
        WriteObject(scriptResult);
        SessionState.PSVariable.Set(Common.ScriptScopeLogicObjectName, previousScriptScopeLogicObject);
    }
}
