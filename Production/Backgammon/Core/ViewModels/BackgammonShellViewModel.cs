namespace Backgammon.Core.ViewModels;
public class BackgammonShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<BackgammonPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}