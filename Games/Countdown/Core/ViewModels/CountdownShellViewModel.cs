namespace Countdown.Core.ViewModels;
public class CountdownShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<CountdownPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}