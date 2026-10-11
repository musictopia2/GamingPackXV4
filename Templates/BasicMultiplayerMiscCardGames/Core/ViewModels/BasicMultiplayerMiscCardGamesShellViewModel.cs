namespace BasicMultiplayerMiscCardGames.Core.ViewModels;
public class BasicMultiplayerMiscCardGamesShellViewModel(IGamePackageResolver main,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<BasicMultiplayerMiscCardGamesPlayerItem>(main, container, gameData, basicData, save, test, aggregator, toast)
{
}