using System;
using System.Management.Automation;
using LibEd.EdData;
using System.Text;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;

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

    protected IEdDataLogicFactory LogicObj { get; set; }

    [Parameter(ValueFromPipeline = true)]
    public PSObject InputObject { get; set; }
    protected List<byte> InputByteList { get; set; }

    protected StringBuilder StringBuilderForInputObject { get; set; }

    protected override void BeginProcessing()
    {
        InputByteList = new();
        StringBuilderForInputObject = new StringBuilder();
        LogicObj = Common.DetermineEdDataLogic(SessionState, EdDataLogicObject, Path);
    }

    protected override void ProcessRecord()
    {
        if(InputObject.BaseObject is string inputString)
        {
            StringBuilderForInputObject.AppendLine(inputString);
            return;
        }
        else if(InputObject.BaseObject is byte inputByte)
        {
            InputByteList.Add(inputByte);
            return;
        }
        throw new InvalidPipelineStateException($"This pipeline input type is not supported. [Type={InputObject.BaseObject.GetType()}]");
    }

    protected override void EndProcessing()
    {
        byte[] bytes;
        if(InputByteList.Count > 0)
        {
            bytes = InputByteList.ToArray();
        }
        else if(StringBuilderForInputObject.Length > 0)
        {
            StringBuilderForInputObject.Remove(StringBuilderForInputObject.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            string combinedString = StringBuilderForInputObject.ToString();
            bytes = Encoding.UTF8.GetBytes(combinedString);
        }
        else
        {
            throw new ArgumentNullException("No input, or entered value is null.");
        }
        LogicObj.CreateWorker().Stash(IndexName, bytes);
    }
}
