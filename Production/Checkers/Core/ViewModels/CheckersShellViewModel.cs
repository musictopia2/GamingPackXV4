namespace Checkers.Core.ViewModels;
public class CheckersShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<CheckersPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}