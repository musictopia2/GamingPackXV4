namespace ClueCardGame.Core.ViewModels;
public class ClueCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<ClueCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}