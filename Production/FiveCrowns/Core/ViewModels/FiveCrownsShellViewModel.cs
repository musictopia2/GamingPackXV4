namespace FiveCrowns.Core.ViewModels;
public class FiveCrownsShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<FiveCrownsPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}