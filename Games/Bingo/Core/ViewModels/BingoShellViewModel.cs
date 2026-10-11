namespace Bingo.Core.ViewModels;
public class BingoShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<BingoPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}