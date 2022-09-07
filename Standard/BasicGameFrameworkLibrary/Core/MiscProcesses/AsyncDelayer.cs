namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public class AsyncDelayer : IAsyncDelayer
{
    private TestOptions? _test;
    private bool GetTester()
    {
        _test ??= Resolver!.Resolve<TestOptions>();
        if (InProgressHelpers.MoveInProgress == false)
        {
            InProgressHelpers.MoveInProgress = true;
        }
        return true;
    }
    async Task IAsyncDelayer.DelayMilli(int howLong)
    {
        if (GetTester() == false)
        {
            return;
        }
        if (_test!.SlowerMoves)
        {
            await Task.Delay(howLong * 3);
            return;
        }
        await Task.Delay(howLong);
    }
    async Task IAsyncDelayer.DelaySeconds(double howLong)
    {
        if (GetTester() == false)
        {
            return;
        }
        if (_test!.SlowerMoves)
        {
            await Task.Delay(TimeSpan.FromSeconds(howLong * 3));
            return;
        }
        await Task.Delay(TimeSpan.FromSeconds(howLong));
    }
    public static void SetDelayer(IAdvancedDIContainer thisMain, ref IAsyncDelayer delays)
    {
        PopulateContainer(thisMain);
        try
        {

            delays = thisMain.MainContainer!.Resolve<IAsyncDelayer>(""); //try this way now.  possible breaking change.
        }
        catch
        {
            delays = new AsyncDelayer(); //its okay to use default.
        }
    }
}