namespace BasicMultiplayerRegularCardGames.Core.ViewModels;
public class BasicMultiplayerRegularCardGamesShellViewModel(IGamePackageResolver main,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<BasicMultiplayerRegularCardGamesPlayerItem>(main, container, gameData, basicData, save, test, aggregator, toast)
{
}