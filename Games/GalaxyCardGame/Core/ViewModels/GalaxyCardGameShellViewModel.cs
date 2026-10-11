namespace GalaxyCardGame.Core.ViewModels;
public class GalaxyCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<GalaxyCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}