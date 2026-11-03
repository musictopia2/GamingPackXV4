namespace Savannah.Core.ViewModels;
public class SavannahShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<SavannahPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}