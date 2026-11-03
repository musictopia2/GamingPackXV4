namespace ConnectFour.Core.ViewModels;
public class ConnectFourShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<ConnectFourPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}