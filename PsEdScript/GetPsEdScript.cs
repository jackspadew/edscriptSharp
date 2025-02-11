using System.Management.Automation;
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
    public SwitchParameter AsByteStream { get; set; }

    [Parameter(
        Mandatory = false
        )]
    public IEdDataLogicFactory EdDataLogicObject { get; set; }

    protected IEdDataLogicFactory LogicObj { get; set; }

    protected override void BeginProcessing()
    {
        LogicObj = Common.DetermineEdDataLogic(SessionState, EdDataLogicObject, Path);
        var plainBytes = LogicObj.CreateWorker().Extract(IndexName);
        if(AsByteStream.IsPresent)
        {
            WriteObject(plainBytes);
            return;
        }
        var plainText = Encoding.UTF8.GetString(plainBytes);
        WriteObject(plainText);
    }
}
