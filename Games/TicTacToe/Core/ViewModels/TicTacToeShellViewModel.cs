namespace TicTacToe.Core.ViewModels;
public class TicTacToeShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<TicTacToePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}