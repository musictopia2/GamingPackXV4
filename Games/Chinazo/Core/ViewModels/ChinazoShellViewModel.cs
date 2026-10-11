namespace Chinazo.Core.ViewModels;
public class ChinazoShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<ChinazoPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}