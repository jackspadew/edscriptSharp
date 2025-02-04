namespace PsEdScript;

using System;
using LibEd.EdData;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public static class Common
{
    public const string ScriptScopeLogicObjectName = "script:PsEdScriptLogic";

    public static void ThrowArgumentNullOrEmptyException(string Name)
    {
        throw new ArgumentException($"The argument {Name} is null or empty.");
    }

    public static string ReadHostPassword()
    {
        using var ps = PowerShell.Create(RunspaceMode.CurrentRunspace);
        ps.AddCommand("Read-Host").AddParameter("Prompt", "password").AddParameter("MaskInput");
        return (string)ps.Invoke<string>()[0];
    }

    public static IEdDataWorker GetEdDataWorker(SessionState sessionState, IEdDataLogicFactory edDataLogicObject, string Path, string password=null)
    {
        if(edDataLogicObject != null)
        {
            return edDataLogicObject.CreateWorker();
        }
        var globalEdDataLogicObject = sessionState.PSVariable.Get(Common.ScriptScopeLogicObjectName);
        if(globalEdDataLogicObject is IEdDataLogicFactory logicObj)
        {
            return logicObj.CreateWorker();
        }
        if(string.IsNullOrWhiteSpace(Path))
        {
            Path = Environment.GetEnvironmentVariable("PsEdScriptDatabasePath");
            if(string.IsNullOrWhiteSpace(Path))
            {
                Common.ThrowArgumentNullOrEmptyException(nameof(Path));
            }
        }
        if(string.IsNullOrWhiteSpace(password))
        {
            password = Common.ReadHostPassword();
            if(string.IsNullOrWhiteSpace(password))
            {
                Common.ThrowArgumentNullOrEmptyException(nameof(password));
            }
        }
        IEdDataLogicFactory EdDataLogicObject = new BasicEdDataLogicFactory(Path, password);
        return EdDataLogicObject.CreateWorker();
    }
}

public enum SupportedScriptLanguage
{
    pwsh,
    python,
}
