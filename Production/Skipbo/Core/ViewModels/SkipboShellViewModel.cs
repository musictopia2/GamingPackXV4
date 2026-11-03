namespace Skipbo.Core.ViewModels;
public class SkipboShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<SkipboPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}