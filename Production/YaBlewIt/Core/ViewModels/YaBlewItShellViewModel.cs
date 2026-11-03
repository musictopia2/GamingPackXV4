namespace YaBlewIt.Core.ViewModels;
public class YaBlewItShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<YaBlewItPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}