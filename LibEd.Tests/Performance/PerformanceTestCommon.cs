namespace LibEd.PerformanceTests;

using System.Diagnostics;

public static class PerformanceTestCommon
{
    public static void CompletesIn(string testName, int targettime, Action action, int timeout=60000)
    {
        var stopwatch = Stopwatch.StartNew();
        var task = Task.Run(action);
        var completedInTime = Task.WaitAll(new[] { task }, TimeSpan.FromMilliseconds(timeout));
        stopwatch.Stop();
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
            Assert.Fail($"The task was not completed due to timeout(={timeout}ms). ");
        }
        string seconds = (stopwatch.ElapsedMilliseconds / 1000.0).ToString("F3");
        string completionTimeMessage = $"completion time: {seconds} sec";
        if (stopwatch.ElapsedMilliseconds > targettime)
        {
            Assert.Fail($"Task completion time exceeds the target time(target time={targettime}ms). ({completionTimeMessage})");
        }
        Console.WriteLine($"[{testName}] {completionTimeMessage}");
    }
}
