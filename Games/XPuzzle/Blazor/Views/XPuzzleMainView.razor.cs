namespace XPuzzle.Blazor.Views;
public partial class XPuzzleMainView
{
    private static XPuzzleCollection GetSpaceList()
    {
        XPuzzleSaveInfo thisSave = Resolver!.Resolve<XPuzzleSaveInfo>();
        return thisSave.SpaceList;
    }
    private async Task ClickedAsync(XPuzzleSpaceInfo space)
    {
        await DataContext!.MakeMoveAsync(space);
        if (DataContext!.CommandContainer.CanExecuteBasics == false)
        {
            return;
        }
        await DataContext.CommandContainer.ProcessCustomCommandAsync(
            () => DataContext.MakeMoveAsync(space));
    }
}