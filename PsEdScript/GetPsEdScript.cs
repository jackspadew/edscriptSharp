using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

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

        using var ps = PowerShell.Create(RunspaceMode.CurrentRunspace);
        ps.AddCommand("Read-Host").AddParameter("Prompt", "password").AddParameter("MaskInput");
        var password = ps.Invoke<string>();

        WriteObject($"IndexName: {IndexName}, Path: {Path}, password: {password}");
    }

    protected virtual void ThrowArgumentNullOrEmptyException(string Name)
    {
        throw new ArgumentException($"The argument {Name} is null or empty.");
    }
}
