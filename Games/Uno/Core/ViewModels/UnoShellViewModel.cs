namespace Uno.Core.ViewModels;
public class UnoShellViewModel : BasicMultiplayerShellViewModel<UnoPlayerItem>
{
    public UnoShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        UnoColorsDelegates delegates,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.OpenColorAsync = OpenColorAsync;
        delegates.CloseColorAsync = CloseColorAsync;
    }
    public ChooseColorViewModel? ColorScreen { get; set; }
    private async Task OpenColorAsync()
    {
        if (IsMainOpened)
        {
            await CloseMainAsync();
        }
        if (IsPromptOpened)
        {
            ClosePrompt();
        }
        await OpenPromptAsync();
    }
    private async Task CloseColorAsync()
    {
        if (IsPromptOpened == false && IsMainOpened)
        {
            return;
        }
        if (IsPromptOpened)
        {
            ClosePrompt();
        }
        await StartNewGameAsync(); //misleading because sometimes it can reload even if not new game like in cases like uno.
    }
}