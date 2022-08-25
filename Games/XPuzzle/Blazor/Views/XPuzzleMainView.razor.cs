namespace XPuzzle.Blazor.Views;
public partial class XPuzzleMainView
{
    private static XPuzzleCollection GetSpaceList()
    {
        XPuzzleSaveInfo thisSave = aa.Resolver!.Resolve<XPuzzleSaveInfo>();
        return thisSave.SpaceList;
    }
    private async Task ClickedAsync(XPuzzleSpaceInfo space)
    {
        if (DataContext!.CommandContainer.CanExecuteBasics() == false)
        {
            return;
        }
        DataContext!.CommandContainer.StartExecuting();
        await DataContext.MakeMoveAsync(space);
        DataContext.CommandContainer.StopExecuting();
    }
}