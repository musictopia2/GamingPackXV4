namespace RollEm.Core.ViewModels;
public class RollEmShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<RollEmPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}