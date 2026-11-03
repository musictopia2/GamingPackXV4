namespace DiceDominos.Core.ViewModels;
public class DiceDominosShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<DiceDominosPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}