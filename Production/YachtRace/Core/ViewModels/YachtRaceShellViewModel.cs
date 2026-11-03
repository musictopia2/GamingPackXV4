namespace YachtRace.Core.ViewModels;
public class YachtRaceShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<YachtRacePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}