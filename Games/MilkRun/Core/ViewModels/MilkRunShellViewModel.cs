namespace MilkRun.Core.ViewModels;
public class MilkRunShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<MilkRunPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}