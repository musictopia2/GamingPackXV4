namespace Fluxx.Core.ViewModels;
public class FluxxShellViewModel : BasicMultiplayerShellViewModel<FluxxPlayerItem>
{
    private readonly FluxxDelegates _delegates;
    private EnumActionScreen _currentAction = EnumActionScreen.None;
    public FluxxShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        FluxxDelegates delegates,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.LoadMainScreenAsync = LoadMainScreenAsync;
        delegates.LoadActionScreenAsync = LoadActionScreenAsync;
        delegates.LoadKeeperScreenAsync = LoadKeeperScreenAsync;
        delegates.CurrentScreen = GetCurrentScreenCategory;
        _delegates = delegates;
    }
    private async Task LoadMainScreenAsync()
    {
        if (IsPromptOpened)
        {
            ClosePrompt(); //i think.
        }
        _currentAction = EnumActionScreen.None; //i think since you are now in main.
        await LoadMainAsync();
        RefreshEnables();
    }
    private EnumActionScreen GetCurrentScreenCategory()
    {
        return _currentAction;
    }
    private async Task LoadActionScreenAsync()
    {
        if (_currentAction == EnumActionScreen.ActionScreen)
        {
            throw new CustomBasicException("Previous action was not loaded.  Rethink");
        }
        if (IsPromptOpened)
        {
            ClosePrompt();
        }
        _currentAction = EnumActionScreen.ActionScreen;
        await CloseMainAsync();
        await OpenPromptAsync();
        RefreshEnables();
    }
    private void RefreshEnables()
    {
        if (_delegates.RefreshEnables == null)
        {
            throw new CustomBasicException("Nobody is refreshing enables.  Rethink");
        }
        _delegates.RefreshEnables.Invoke();
    }
    private async Task LoadKeeperScreenAsync()
    {
        if (_currentAction == EnumActionScreen.KeeperScreen)
        {
            return; //already loaded.  no need for error.
        }
        if (IsPromptOpened)
        {
            ClosePrompt();
        }
        _currentAction = EnumActionScreen.KeeperScreen;
        await CloseMainAsync();
        await OpenPromptAsync();   
    }
}