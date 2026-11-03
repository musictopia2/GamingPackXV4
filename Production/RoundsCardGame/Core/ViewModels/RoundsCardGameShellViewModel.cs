namespace RoundsCardGame.Core.ViewModels;
public class RoundsCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<RoundsCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}