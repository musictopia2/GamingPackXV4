namespace DominoBonesMultiplayerGames.Core.ViewModels;
public class DominoBonesMultiplayerGamesShellViewModel(IGamePackageResolver main,
    CommandContainer command,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<DominoBonesMultiplayerGamesPlayerItem>(main, command, gameData, basicData, save, test, aggregator, toast)
{
    
}