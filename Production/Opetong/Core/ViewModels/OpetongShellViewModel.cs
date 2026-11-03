namespace Opetong.Core.ViewModels;
public class OpetongShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<OpetongPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}