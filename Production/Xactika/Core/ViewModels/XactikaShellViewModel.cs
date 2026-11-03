namespace Xactika.Core.ViewModels;
public class XactikaShellViewModel : BasicTrickShellViewModel<XactikaPlayerItem>
{
    public XactikaShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        XactikaDelegates delegates,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.LoadModeAsync = LoadModeAsync;
        delegates.CloseModeAsync = CloseModeAsync;
    }
    
    
    private async Task LoadModeAsync()
    {
        if (IsPromptOpened)
        {
            return;
        }
        if (IsMainOpened)
        {
            throw new CustomBasicException("Cannot close game from mode.  Rethink");
        }
        await OpenPromptAsync();
    }
    private async Task CloseModeAsync()
    {
        if (IsPromptOpened)
        {
            ClosePrompt();
        }

        if (IsMainOpened == false)
        {
            await StartNewGameAsync();
        }
    }
    //private async Task CloseModeAsync()
    //{
    //    if (IsPromptOpened == false)
    //    {
    //        return;
    //    }
    //    ClosePrompt();
    //    await StartNewGameAsync();
    //}   
}