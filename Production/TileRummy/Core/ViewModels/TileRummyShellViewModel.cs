namespace TileRummy.Core.ViewModels;
public class TileRummyShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<TileRummyPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}