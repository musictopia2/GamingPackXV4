namespace YahtzeeHandsDown.Core.ViewModels;
public class YahtzeeHandsDownShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<YahtzeeHandsDownPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}