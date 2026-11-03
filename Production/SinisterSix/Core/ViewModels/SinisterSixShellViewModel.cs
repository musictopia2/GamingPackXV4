namespace SinisterSix.Core.ViewModels;
public class SinisterSixShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<SinisterSixPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}