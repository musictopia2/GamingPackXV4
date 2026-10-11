namespace MonasteryCardGame.Core.ViewModels;
public class MonasteryCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<MonasteryCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}