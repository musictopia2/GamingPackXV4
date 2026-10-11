namespace Mancala.Core.ViewModels;
public class MancalaShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<MancalaPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}