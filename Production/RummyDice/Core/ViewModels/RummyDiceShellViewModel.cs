namespace RummyDice.Core.ViewModels;
public class RummyDiceShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<RummyDicePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}