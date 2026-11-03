namespace CousinRummy.Core.ViewModels;
public class CousinRummyShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<CousinRummyPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}