namespace BladesOfSteel.Core.ViewModels;
public class BladesOfSteelShellViewModel : BasicMultiplayerShellViewModel<BladesOfSteelPlayerItem>
{
    public BladesOfSteelShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        BladesOfSteelScreenDelegates delegates,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.ReloadFaceoffAsync = LoadFaceoffAsync;
        delegates.LoadMainGameAsync = LoadMainGameAsync;
    }
    private async Task LoadMainGameAsync()
    {
        ClosePrompt();
        await base.StartNewGameAsync(); //i think.
    }
    private async Task LoadFaceoffAsync()
    {
        await CloseMainAsync();
        await OpenPromptAsync();
    }
    protected override async Task StartNewGameAsync()
    {
        await CloseMainAsync(); //to start with.
        BladesOfSteelGameContainer gameContainer = Resolver!.Resolve<BladesOfSteelGameContainer>();
        if (gameContainer.SaveRoot is null || gameContainer.SaveRoot.IsFaceOff)
        {
            if (IsPromptOpened)
            {
                ClosePrompt();
            }
            await OpenPromptAsync();
            return;
        }
        await base.StartNewGameAsync();
    }
}