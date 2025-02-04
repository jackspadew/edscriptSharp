using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using LibEd.EdData;

namespace PsEdScript;

[Cmdlet(VerbsCommon.Get,"PsEdScript")]
[OutputType(typeof(string))]
public class GetPsEdScript : PSCmdlet
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

        WriteObject($"IndexName: {IndexName}, Path: {Path}, password: {password}");
    }

    protected virtual void ThrowArgumentNullOrEmptyException(string Name)
    {
        throw new ArgumentException($"The argument {Name} is null or empty.");
    }
}
