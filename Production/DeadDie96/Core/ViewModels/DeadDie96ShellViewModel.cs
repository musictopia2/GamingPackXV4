namespace DeadDie96.Core.ViewModels;
public class DeadDie96ShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<DeadDie96PlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}