namespace SnagCardGame.Core.ViewModels;
public class SnagCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<SnagCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}