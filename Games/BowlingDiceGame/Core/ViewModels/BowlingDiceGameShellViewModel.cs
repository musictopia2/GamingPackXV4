namespace BowlingDiceGame.Core.ViewModels;
public class BowlingDiceGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<BowlingDiceGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}