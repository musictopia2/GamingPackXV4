namespace SnakesAndLadders.Core.ViewModels;
public class SnakesAndLaddersShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<SnakesAndLaddersPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}