namespace BasicMultiplayerTrickCardGames.Core.ViewModels;
public class BasicMultiplayerTrickCardGamesShellViewModel(IGamePackageResolver main,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicTrickShellViewModel<BasicMultiplayerTrickCardGamesPlayerItem>(main, container, gameData, basicData, save, test, aggregator, toast)
{
}