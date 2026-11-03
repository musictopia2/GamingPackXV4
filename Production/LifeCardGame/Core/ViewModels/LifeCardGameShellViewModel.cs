namespace LifeCardGame.Core.ViewModels;
public class LifeCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<LifeCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}