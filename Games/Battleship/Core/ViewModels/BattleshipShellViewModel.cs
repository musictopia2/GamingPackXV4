namespace Battleship.Core.ViewModels;
public class BattleshipShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<BattleshipPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}