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
    public XactikaModeViewModel? ModeScreen { get; set; }
    protected override async Task GetStartingScreenAsync()
    {

        await LoadModeAsync();
    }
    private async Task LoadModeAsync()
    {
        if (ModeScreen != null)
        {
            return;
        }
        if (MainVM != null)
        {
            await CloseMainAsync("Cannot close game from mode.  Rethink");
        }
        ModeScreen = MainContainer.Resolve<XactikaModeViewModel>();
        await LoadScreenAsync(ModeScreen);
    }
    private async Task CloseModeAsync()
    {
        if (ModeScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(ModeScreen);
        ModeScreen = null;
        await StartNewGameAsync();
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<XactikaMainViewModel>();
        return model;
    }
}