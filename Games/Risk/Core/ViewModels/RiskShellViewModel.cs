namespace Risk.Core.ViewModels;
public class RiskShellViewModel : BasicBoardGamesShellViewModel<RiskPlayerItem>
{
    private readonly IMessageBox _message;
    public RiskShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast,
        IMessageBox message
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        _message = message;
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<RiskMainViewModel>();
        return model;
    }
    protected override async Task ShowNewGameAsync()
    {
        await _message.ShowMessageAsync("Game is over.  However, unable to allow for new game.  If you want new game, close out and reconnect again.");
    }
}