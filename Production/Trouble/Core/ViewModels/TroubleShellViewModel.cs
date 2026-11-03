namespace Trouble.Core.ViewModels;
public class TroubleShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<TroublePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}