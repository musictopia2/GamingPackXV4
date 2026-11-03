namespace Spades4Player.Core.ViewModels;
public class Spades4PlayerShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<Spades4PlayerPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}