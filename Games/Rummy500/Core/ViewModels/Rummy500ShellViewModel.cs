namespace Rummy500.Core.ViewModels;
public class Rummy500ShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<Rummy500PlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}