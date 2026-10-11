namespace Candyland.Core.ViewModels;
public class CandylandShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<CandylandPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}