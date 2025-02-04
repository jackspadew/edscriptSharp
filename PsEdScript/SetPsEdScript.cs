using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using LibEd.EdData;
using System.Text;

namespace PsEdScript;

[Cmdlet(VerbsCommon.Set,"PsEdScript")]
public class SetPsEdScript : PSCmdlet
{
    [Parameter(
        Mandatory = true
        )]
    public string IndexName { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public string TargetPath { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public string Path { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public IEdDataLogicFactory EdDataLogicObject { get; set; }

    [Parameter(ValueFromPipeline = true)]
    public string[] InputStrings { get; set; }
    public string CombinedInputStrings { get; set; }

    protected override void BeginProcessing()
    {
        if(string.IsNullOrWhiteSpace(IndexName))
        {
            ThrowArgumentNullOrEmptyException(nameof(IndexName));
        }

        if(string.IsNullOrWhiteSpace(Path)) Path = Environment.GetEnvironmentVariable("PsEdScriptDatabasePath");
        if(string.IsNullOrWhiteSpace(Path))
        {
            ThrowArgumentNullOrEmptyException(nameof(Path));
        }

        if(EdDataLogicObject == null)
        {
            var tmpObj = SessionState.PSVariable.Get(Common.ScriptScopeLogicObjectName);
            if(tmpObj is IEdDataLogicFactory logicObj)
            {
                EdDataLogicObject = logicObj;
            }
        }

        using var ps = PowerShell.Create(RunspaceMode.CurrentRunspace);
        ps.AddCommand("Read-Host").AddParameter("Prompt", "password").AddParameter("MaskInput");
        string password = (string)ps.Invoke<string>()[0];

        if(EdDataLogicObject == null)
        {
            EdDataLogicObject = new BasicEdDataLogicFactory(Path, password);
        }
    }

    protected override void ProcessRecord()
    {
        if (InputStrings != null)
        {
            CombinedInputStrings = string.Join(Environment.NewLine, InputStrings);
        }
    }

    protected override void EndProcessing()
    {
        var bytes = Encoding.UTF8.GetBytes(CombinedInputStrings);
        var worker = EdDataLogicObject.CreateWorker();
        worker.Stash(IndexName, bytes);
    }

    protected virtual void ThrowArgumentNullOrEmptyException(string Name)
    {
        throw new ArgumentException($"The argument {Name} is null or empty.");
    }
}
