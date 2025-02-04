using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using LibEd.EdData;
using System.Text;

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

    protected IEdDataWorker Worker { get; set; }

    protected override void BeginProcessing()
    {
        Worker = Common.GetEdDataWorker(SessionState, EdDataLogicObject, Path);
        var plainBytes = Worker.Extract(IndexName);
        var plainText = Encoding.UTF8.GetString(plainBytes);
        WriteObject(plainText);
    }
}
