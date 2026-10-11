namespace FillOrBust.Core.ViewModels;
public class FillOrBustShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<FillOrBustPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}