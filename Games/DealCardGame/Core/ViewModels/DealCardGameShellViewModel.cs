namespace DealCardGame.Core.ViewModels;
public class DealCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<DealCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}