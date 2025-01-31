namespace LibEd.Tests;

public static class CommonFunctions
{
    public static void DeleteFileIfExists(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void CompletesIn(int timeout, Action action)
    {
        var task = Task.Run(action);
        var completedInTime = Task.WaitAll(new[] { task }, TimeSpan.FromMilliseconds(timeout));
        if (task.Exception != null)
        {
            if (task.Exception.InnerExceptions.Count == 1)
            {
                throw task.Exception.InnerExceptions[0];
            }
            throw task.Exception;
        }
        if (!completedInTime)
        {
            throw new TimeoutException($"Task did not complete in {timeout} ms.");
        }
    }
}
