namespace BattleshipLite.Core.ViewModels;
public class BattleshipLiteShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<BattleshipLitePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}