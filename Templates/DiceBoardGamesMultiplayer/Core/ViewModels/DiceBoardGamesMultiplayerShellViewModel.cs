namespace DiceBoardGamesMultiplayer.Core.ViewModels;
public class DiceBoardGamesMultiplayerShellViewModel(IGamePackageResolver main,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BeginningColorShellViewModel<DiceBoardGamesMultiplayerPlayerItem>(main, container, gameData, basicData, save, test, aggregator, toast)
{
}