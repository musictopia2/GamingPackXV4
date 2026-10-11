namespace ConnectTheDots.Core.ViewModels;
public class ConnectTheDotsShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<ConnectTheDotsPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}