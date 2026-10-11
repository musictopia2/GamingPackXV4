namespace SorryCardGame.Core.ViewModels;
public class SorryCardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<SorryCardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}