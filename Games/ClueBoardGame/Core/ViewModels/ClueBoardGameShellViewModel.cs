namespace ClueBoardGame.Core.ViewModels;
public class ClueBoardGameShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicBoardGamesShellViewModel<ClueBoardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<ClueBoardGameMainViewModel>();
        return model;
    }
}