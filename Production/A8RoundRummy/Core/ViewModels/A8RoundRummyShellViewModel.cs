namespace A8RoundRummy.Core.ViewModels;
public class A8RoundRummyShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<A8RoundRummyPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}