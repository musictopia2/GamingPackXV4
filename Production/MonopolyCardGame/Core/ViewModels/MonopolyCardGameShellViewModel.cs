namespace MonopolyCardGame.Core.ViewModels;
public class MonopolyCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<MonopolyCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}