namespace Chess.Core.ViewModels;
public class ChessShellViewModel : BasicBoardGamesShellViewModel<ChessPlayerItem>
{
    public ChessShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<ChessMainViewModel>();
        return model;
    }
}