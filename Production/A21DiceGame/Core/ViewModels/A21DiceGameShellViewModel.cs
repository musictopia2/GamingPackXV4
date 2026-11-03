namespace A21DiceGame.Core.ViewModels;
public class A21DiceGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<A21DiceGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}