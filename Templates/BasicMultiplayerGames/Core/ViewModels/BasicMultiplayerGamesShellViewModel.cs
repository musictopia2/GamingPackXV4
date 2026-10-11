namespace BasicMultiplayerGames.Core.ViewModels;
public class BasicMultiplayerGamesShellViewModel(IGamePackageResolver main,
    CommandContainer command,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<BasicMultiplayerGamesPlayerItem>(main, command, gameData, basicData, save, test, aggregator, toast)
{
}