namespace Spades2Player.Core.ViewModels;
public class Spades2PlayerShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<Spades2PlayerPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}