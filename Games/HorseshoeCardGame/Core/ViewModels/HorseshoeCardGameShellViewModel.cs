namespace HorseshoeCardGame.Core.ViewModels;
public class HorseshoeCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<HorseshoeCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}