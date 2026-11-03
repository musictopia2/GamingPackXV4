namespace CoveredUp.Core.ViewModels;
public class CoveredUpShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<CoveredUpPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}