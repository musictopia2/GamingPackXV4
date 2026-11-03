namespace Pinochle2Player.Core.ViewModels;
public class Pinochle2PlayerShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<Pinochle2PlayerPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{

}