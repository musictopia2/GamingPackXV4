namespace DominosRegular.Core.ViewModels;
public class DominosRegularShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<DominosRegularPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}