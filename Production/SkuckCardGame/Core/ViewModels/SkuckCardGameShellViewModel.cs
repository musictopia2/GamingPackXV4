namespace SkuckCardGame.Core.ViewModels;
public class SkuckCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<SkuckCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}