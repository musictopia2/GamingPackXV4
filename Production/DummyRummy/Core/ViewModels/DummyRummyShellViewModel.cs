namespace DummyRummy.Core.ViewModels;
public class DummyRummyShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<DummyRummyPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}