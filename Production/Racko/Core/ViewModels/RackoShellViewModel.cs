namespace Racko.Core.ViewModels;
public class RackoShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<RackoPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}