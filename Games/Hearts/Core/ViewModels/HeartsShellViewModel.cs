namespace Hearts.Core.ViewModels;
public class HeartsShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<HeartsPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}