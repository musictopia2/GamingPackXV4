namespace Aggravation.Core.ViewModels;
public class AggravationShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<AggravationPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}