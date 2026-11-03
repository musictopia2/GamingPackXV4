namespace Rook.Core.ViewModels;
public class RookShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<RookPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}