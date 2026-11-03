namespace GoFish.Core.ViewModels;
public class GoFishShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<GoFishPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}