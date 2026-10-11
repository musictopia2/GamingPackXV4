namespace PlainBoardGamesMultiplayer.Core.ViewModels;
public class PlainBoardGamesMultiplayerShellViewModel(IGamePackageResolver main,
    CommandContainer command,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BeginningColorShellViewModel<PlainBoardGamesMultiplayerPlayerItem>(main, command, gameData, basicData, save, test, aggregator, toast)
{
}