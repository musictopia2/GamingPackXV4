namespace Payday.Core.ViewModels;
public class PaydayShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<PaydayPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}