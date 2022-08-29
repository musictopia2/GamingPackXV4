namespace Payday.Core.ViewModels;
public class PaydayShellViewModel : BasicBoardGamesShellViewModel<PaydayPlayerItem>
{
    //private readonly IMessageBox _message;
    public PaydayShellViewModel(IGamePackageResolver mainContainer,
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
        //_message = message;
    }
    //protected override void ReplaceVMData()
    //{
    //    //ignore but reserves the right to do something else if needed.
    //}
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<PaydayMainViewModel>();
        return model;
    }
    //protected override async Task ShowNewGameAsync()
    //{
    //    await _message.ShowMessageAsync("Game is over.  However, unable to allow for new game.  If you want new game, close out and reconnect again.");
    //}
}