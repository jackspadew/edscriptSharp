namespace PsEdScript;

using System;
using System.Text;
using LibEd.EdData;
using System.Management.Automation;
using PsEdScript.Parser;

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

    public static IEdDataLogicFactory DetermineEdDataLogic(SessionState sessionState, IEdDataLogicFactory specifiedLogicObj, string Path, string password=null)
    {
        // if argument "EdDataLogicObject" is specified directly, use it.
        if(specifiedLogicObj != null)
        {
            return specifiedLogicObj;
        }
        // Otherwise, use the global variable if it is set.
        var globalAnyTypeObject = sessionState.PSVariable.Get(Common.ScriptScopeLogicObjectName);
        if(globalAnyTypeObject is IEdDataLogicFactory globalLogicObj)
        {
            return globalLogicObj;
        }
        // Otherwise, use the one generated in the cmdlet.
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
        IEdDataLogicFactory generatedLogicObj = new BasicEdDataLogicFactory(Path, password);
        return generatedLogicObj;
    }

    public static object InvokeScriptByByteArray(byte[] scriptTextBytes)
    {
        string firstLine = GetFirstLineFromByteArray(scriptTextBytes);
        var parser = ShebangParser.Parse(firstLine);
        if(!parser.IsShebang) throw new Exception("The first line of script text is not match Shebang format.");
        var scriptText = Encoding.UTF8.GetString(scriptTextBytes);
        if(parser.CommandExact == "pwsh")
        {
            return InvokePowershellScriptByString(scriptText);
        }
        else if(parser.CommandExact == "python")
        {
            return InvokePythonScript(scriptText);
        }
        throw new Exception("This is not any valid script text.");
    }

    public static object InvokePowershellScriptByString(string ps1Text)
    {
        using (PowerShell ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
        {
            ps.AddScript(ps1Text);
            return ps.Invoke();
        }
    }

    public static object InvokePythonScript(string pyText)
    {
        using (PowerShell ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
        {
            ps.AddScript("$args[0] | python -");
            ps.AddArgument(pyText);
            return ps.Invoke();
        }
    }

    public static string GetFirstLineFromByteArray(byte[] byteArray)
    {
        int length = Math.Min(byteArray.Length, 124);
        byte[] limitedArray = new byte[length];
        Array.Copy(byteArray, limitedArray, length);
        string result = Encoding.UTF8.GetString(limitedArray);
        int newlineIndex = result.IndexOf('\n');
        if (newlineIndex != -1)
        {
            return result.Substring(0, newlineIndex);
        }
        throw new InvalidOperationException("The byte array does not contain valid string data.");
    }
}
