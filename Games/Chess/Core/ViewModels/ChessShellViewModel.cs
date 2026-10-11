namespace Chess.Core.ViewModels;
public class ChessShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<ChessPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}