using System;
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

    [Parameter(
        ValueFromRemainingArguments = true
        )]
    public object[] RemainingArguments { get; set; } = new object[]{};

    protected IEdDataLogicFactory LogicObj { get; set; }
    protected object previousScriptScopeLogicObject { get; set; }

    protected override void BeginProcessing()
    {
        previousScriptScopeLogicObject = SessionState.PSVariable.GetValue(Common.ScriptScopeLogicObjectName);
        try
        {
            LogicObj = Common.DetermineEdDataLogic(SessionState, EdDataLogicObject, Path);
            SessionState.PSVariable.Set(Common.ScriptScopeLogicObjectName, LogicObj);
            var plainBytes = LogicObj.CreateWorker().Extract(IndexName);
            var scriptResult = Common.InvokeScriptByByteArray(plainBytes);
            WriteObject(scriptResult);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
        finally
        {
            SessionState.PSVariable.Set(Common.ScriptScopeLogicObjectName, previousScriptScopeLogicObject);
        }
    }
}
