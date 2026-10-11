namespace Sorry.Core.ViewModels;
public class SorryShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<SorryPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}