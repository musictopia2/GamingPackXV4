namespace OldMaid.Core.ViewModels;
public class OldMaidShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<OldMaidPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}