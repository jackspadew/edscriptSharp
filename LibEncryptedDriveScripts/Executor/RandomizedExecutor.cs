
namespace LibEncryptedDriveScripts.Executor;

public class RandomizedExecutor : IActionExecutor
{
    protected int[] ExecutionCountArray;

    public RandomizedExecutor(int[] executionCountArray)
    {
        ExecutionCountArray = executionCountArray;
    }
    public void Run(Action[] actions)
    {
        List<Action> actionList = CreateActionList(ExecutionCountArray, actions);
        var randomizedList = actionList.OrderBy(a => Guid.NewGuid()).ToList();
        foreach(var action in randomizedList)
        {
            action();
        }
    }

    protected List<Action> CreateActionList(int[] executionCountArray, Action[] actions)
    {
        List<Action> actionList = new();
        int currentIndex = 0;
        foreach(var act in actions)
        {
            int executionCount = (currentIndex < ExecutionCountArray.Length) ? ExecutionCountArray[currentIndex] : 0;
            for(int i=0; i<executionCount; i++)
            {
                actionList.Add(act);
            }
            currentIndex++;
        }
        return actionList;
    }
}
