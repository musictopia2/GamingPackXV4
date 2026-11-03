namespace CaliforniaJack.Core.ViewModels;
public class CaliforniaJackShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<CaliforniaJackPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}