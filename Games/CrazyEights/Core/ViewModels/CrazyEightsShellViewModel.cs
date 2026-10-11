namespace CrazyEights.Core.ViewModels;
public class CrazyEightsShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<CrazyEightsPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}