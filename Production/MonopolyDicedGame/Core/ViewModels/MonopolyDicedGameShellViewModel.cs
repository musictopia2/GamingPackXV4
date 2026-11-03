namespace MonopolyDicedGame.Core.ViewModels;
public class MonopolyDicedGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<MonopolyDicedGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}