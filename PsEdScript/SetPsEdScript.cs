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

    protected IEdDataWorker Worker { get; set; }

    [Parameter(ValueFromPipeline = true)]
    public string[] InputStrings { get; set; }
    public string CombinedInputStrings { get; set; }

    protected override void BeginProcessing()
    {
        Worker = Common.GetEdDataWorker(SessionState, EdDataLogicObject, Path);
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
        Worker.Stash(IndexName, bytes);
    }
}
